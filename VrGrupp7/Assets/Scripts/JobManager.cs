using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class WantedAttribute
{
    public IAttribute Attribute;
    public float potency;
    public float marginOfError;

    public bool CheckIfWanted(float checkPotency)
    {
        return (checkPotency >= potency - marginOfError) && (checkPotency <= potency + marginOfError);

    }
}

public class JobManager : MonoBehaviour
{
    public int turn_in_correct = 0;  // 0= null 1=correct 2=Incorrect
    [SerializeField] TextDisplayer displayer;
    [SerializeField] int startBatch = 1;
    [Range(0, 1)]
    [SerializeField] float minPotency = 0.05f;
    [Range(0, 1)]
    [SerializeField] float maxPotency = 0.75f;
    [SerializeField] GameObject[] rewards;
    [SerializeField] Hatch hatch;
    [Header("Turn in area")]
    [SerializeField] Vector3 turnInPos;
    [SerializeField] Vector3 turnInSize;

    [Header("Debug")]
    [SerializeField] bool AlwaysTurnInCorrect;

    //make it so it's not 33.3333333333 and 66.6666666 so there looks like its no room for error
    const float SAFETY_PERCENT = 0.005f;

    XRGrabInteractable interactable;
    Rigidbody rbody;
    bool hasStarted;
    bool[] isCompleted;
    int correctAmmount = 0;

    IAttribute[] allAttributes;
    [HideInInspector] public List<WantedAttribute> wantedAtributes;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].SetActive(false);
        }

        allAttributes = GetComponents<IAttribute>();
        CreateNewBatch(startBatch);
    }

    [ContextMenu("Get New Attributes")]
    void GetNewBatch()
    {
        CreateNewBatch(startBatch + correctAmmount);
    }

    public void EpicCheat()
    {
        if (correctAmmount > 10)
            return;

        for (int i = 0; i < 6; i++)
        {
            EnableReward();
        }
    }


    void CreateNewBatch(int amount)
    {
        wantedAtributes = new List<WantedAttribute>();

        amount = amount > allAttributes.Length ? allAttributes.Length : amount;

        for (int i = 0; i < amount; i++)
        {
            AddWantedAttribute();
        }

        //Call write text to update textscreen
        Invoke(nameof(WriteText), 5);
    }

    void WriteText()
    {
        displayer.WriteText(FormatJobString());
    }

    bool HasAttribute(IAttribute attribute)
    {
        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            if (wantedAtributes[i].Attribute.GetType() == attribute.GetType())
                return true;
        }

        return false;
    }

    void AddWantedAttribute()
    {
        WantedAttribute newWanted = new WantedAttribute();
        float fullPotency = GetFullPotency();
        if (ConvertPotencyToMaxValue(fullPotency) < minPotency)
        {
            Debug.Log("Full potency reached in jobmanager");
            return;
        }

        int counter = 0;
        int attributeNum = 0;
        while (counter < (allAttributes.Length * 2))
        {
            attributeNum = Random.Range(0, allAttributes.Length);
            if (HasAttribute(allAttributes[attributeNum]) == false)
                break;
            counter++;
        }

        if (counter >= (allAttributes.Length * 2))
        {
            Debug.LogError("WENT OVER ITERATION LIMIT");
            return;
        }

        if (!CheckForException(allAttributes[attributeNum]))
        {
            newWanted.Attribute = allAttributes[attributeNum];
            float margin = Random.Range(2, 5 + 1);
            newWanted.marginOfError = margin;
            float potency = Random.Range(minPotency, fullPotency > 0 ? ConvertPotencyToMaxValue(fullPotency) : maxPotency);
            newWanted.potency = Mathf.RoundToInt(potency * 100.0f) * 0.01f;
            wantedAtributes.Add(newWanted);
        }
        else
        {
            CreateAttributeException(allAttributes[attributeNum]);
        }
    }
    bool CheckForException(IAttribute wanted)
    {
        switch (wanted.GetType().ToString())
        {
            case nameof(PotionType.Bouncy):
                return true;
            case nameof(PotionType.Explosive):
                return true;
            default:
                return false;
        }
    }

    float GetFullPotency()
    {
        float potency = 0;

        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            potency += wantedAtributes[i].potency;
        }

        return potency;
    }

    static float ConvertPotencyToMaxValue(float potency)
    {
        return 1 - (potency + SAFETY_PERCENT);
    }

    void CreateAttributeException(IAttribute attribute)
    {
        WantedAttribute wanted = new WantedAttribute();
        switch (attribute.GetType().ToString())
        {
            case nameof(PotionType.Bouncy):
                wanted.Attribute = attribute;
                wanted.potency = 0;
                wanted.marginOfError = 1000;
                break;
            case nameof(PotionType.Explosive):
                wanted.Attribute = attribute;
                wanted.potency = 0;
                wanted.marginOfError = 1000;
                break;
            default:
                Debug.LogError("TRIED TO CREATE EXEPTION FOR " + attribute.GetType().ToString() + " THAT IS NOT AN EXCEPTION OR NOT IS NOT CORRECTLY IMPLEMENTED", this);
                break;
        }

        if (wanted != null)
            wantedAtributes.Add(wanted);
    }

    string FormatExceptionString(IAttribute Exception)
    {
        switch (Exception.GetType().ToString())
        {
            case nameof(PotionType.Bouncy):
                return "Bouncy";
            case nameof(PotionType.Explosive):
                return "Explosive";
            default:
                Debug.LogError("TRIED TO CREATE EXCEPTION STRING FOR " + Exception.GetType().ToString() + ". THERE IS NO EXCEPTION IMPLEMENTED FOR IT OR IT SHOULD NOT EAVEN HAVE GOTTEN HERE", this);
                return "";
        }
    }

    string FormatJobString()
    {
        if (AlwaysTurnInCorrect)
            return "EVERYTHING WILL BECORRECT. <color=red>DEBUG MODE IS ON<</color>";

        string newJobString = "I want a potion that has ";
        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            if (wantedAtributes.Count > 1)
            {
                if (i == wantedAtributes.Count - 1)
                    newJobString += " and ";
                else if (i != 0)
                    newJobString += ", ";
            }

            if (!CheckForException(wantedAtributes[i].Attribute))
            {
                newJobString += wantedAtributes[i].Attribute.GetName() + " with " + (wantedAtributes[i].potency * 100).ToString() + "% potency";
            }
            else
            {
                if (wantedAtributes.Count > 1)
                    if (i == wantedAtributes.Count - 1)
                        newJobString += " is ";

                newJobString += FormatExceptionString(wantedAtributes[i].Attribute);
            }
        }

        return newJobString;
    }

    #region Turn in logic

    public void TurnIn()
    {
        if (hasStarted)
            return;

        isCompleted = new bool[wantedAtributes.Count];
        Collider[] hitColliders = Physics.OverlapBox(turnInPos, turnInSize);
        LiquidContainer container = null;
        rbody = null;
        interactable = null;

        for (int i = 0; i < hitColliders.Length; i++)
        {

            if (hitColliders[i].gameObject.TryGetComponent(out interactable) == true)
                if (interactable.isSelected)
                    break;

            if (hitColliders[i].gameObject.TryGetComponent(out container) == true)
                break;

        }

        if (container == null)
            return;

        if (container.TryGetComponent(out rbody) == true)
            rbody.isKinematic = true;

        hasStarted = true;
        interactable.enabled = false;

        IAttribute[] containerAttributes = container.GetComponents<IAttribute>();

        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            for (int x = 0; x < containerAttributes.Length; x++)
            {
                if (wantedAtributes[i].Attribute.GetType() == containerAttributes[x].GetType())
                {
                    isCompleted[i] = wantedAtributes[i].CheckIfWanted(containerAttributes[x].GetPotency());
                }
            }
        }

        bool isCorrect = true;

        if (!AlwaysTurnInCorrect)
        {
            for (int i = 0; i < isCompleted.Length; i++)
            {
                if (!isCompleted[i])
                {
                    isCorrect = false;
                    break;
                }
            }
        }

        hatch.StartSequence(container.gameObject, isCorrect);
    }

    void EnableReward()
    {
        if (correctAmmount >= rewards.Length)
            return;

        correctAmmount++;

        rewards[correctAmmount - 1].SetActive(true);

    }

    public void ResetTurnIn()
    {
        hasStarted = false;
    }

    public void TurnInCorrect()
    {
        EnableReward();
        displayer.WriteText("Thank you so much");
        CancelInvoke(nameof(GetNewBatch));
        Invoke(nameof(GetNewBatch), 10);

    }

    public void TurnInIncorrect()
    {
        if (interactable != null)
            interactable.enabled = true;

        if (rbody != null)
            rbody.isKinematic = false;


        string explination = "This was not what i ordered! I ordered a potion with ";
        for (int i = 0; i < isCompleted.Length; i++)
        {
            if (!isCompleted[i])
            {
                if (i != 0)
                    explination += " and ";

                explination += " " + wantedAtributes[i].Attribute.GetName() + " should be " + wantedAtributes[i].potency + "%";
            }
        }

        displayer.WriteText(explination);
        CancelInvoke(nameof(WriteText));
        Invoke(nameof(WriteText), 10);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireCube(turnInPos, turnInSize * 2);
    }
}
