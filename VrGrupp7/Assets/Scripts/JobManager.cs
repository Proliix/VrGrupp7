using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WantedAttribute
{
    public IAttribute Attribute;
    public float potency;
    public bool wantGreater;
}

public class JobManager : MonoBehaviour
{
    [SerializeField] TextDisplayer displayer;
    [SerializeField] int startBatch = 2;
    [Header("Turn in area")]
    [SerializeField] Vector3 turnInPos;
    [SerializeField] Vector3 turnInSize;


    IAttribute[] allAttributes;
    [HideInInspector] public List<WantedAttribute> wantedAtributes;


    // Start is called before the first frame update
    void Start()
    {
        allAttributes = GetComponents<IAttribute>();
        GetNewBatch(startBatch);
    }

    [ContextMenu("Get New Attributes")]
    void NewBatchEditor()
    {
        GetNewBatch(startBatch);
    }

    void GetNewBatch(int amount)
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

        newWanted.Attribute = allAttributes[attributeNum];
        bool wantGreater = Random.Range(0, 2) > 0;
        newWanted.wantGreater = wantGreater;
        float potency = Random.Range(wantGreater ? 0f : 0.1f, wantGreater ? 0.9f : 1f);
        newWanted.potency = Mathf.Floor(Mathf.Round(potency * 100.0f)) * 0.01f;
        wantedAtributes.Add(newWanted);
    }

    string FormatJobString()
    {
        string newJobString = "I want a potion that has ";
        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            if (i == wantedAtributes.Count - 1)
                newJobString += " and ";
            else if (i != 0)
                newJobString += ", ";

            newJobString += wantedAtributes[i].Attribute.GetName() + " with " + (wantedAtributes[i].wantGreater ? "more " : "less ") +
                "than " + (wantedAtributes[i].potency * 100).ToString() + "% potency";
        }

        return newJobString;
    }

    #region Turn in logic

    public void TurnIn()
    {
        bool[] isCompleted = new bool[wantedAtributes.Count];
        Collider[] hitColliders = Physics.OverlapBox(turnInPos, turnInSize);
        LiquidContainer container = null;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.TryGetComponent(out container) == true)
                break;
        }

        if (container == null)
            return;

        IAttribute[] containerAttributes = container.GetComponents<IAttribute>();

        for (int i = 0; i < wantedAtributes.Count; i++)
        {
            for (int x = 0; x < containerAttributes.Length; x++)
            {
                if (wantedAtributes[i].Attribute.GetType() == containerAttributes[x].GetType())
                {
                    switch (wantedAtributes[i].wantGreater)
                    {
                        case true:
                            if (wantedAtributes[i].potency >= containerAttributes[x].GetPotency())
                                isCompleted[i] = true;
                            break;
                        case false:
                            if (wantedAtributes[i].potency <= containerAttributes[x].GetPotency())
                                isCompleted[i] = true;
                            break;
                    }
                }
            }
        }


        for (int i = 0; i < isCompleted.Length; i++)
        {
            if (!isCompleted[i])
            {
                TurnInIncorrect();
                return;
            }
        }

        TurnInCorrect();

    }

    void TurnInCorrect()
    {
        displayer.WriteText("Thank you so much");
        Invoke(nameof(NewBatchEditor), 10);
    }

    void TurnInIncorrect()
    {
        displayer.WriteText("This was not what i ordered!?!?");
        Invoke(nameof(WriteText), 10);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireCube(turnInPos, turnInSize * 2);
    }
}