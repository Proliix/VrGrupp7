using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LiquidContainer))]

public class LiquidCatcher : MonoBehaviour
{
    [SerializeField] GameObject liquidObj;
    [SerializeField] float liquidAddAmount = 0.1f;

    //Material mat;
    LiquidEffect liquid;
    float targetAmount = -10;
    float fillAmount;
    float incomingLiquid = 0;

    Color fadeColorSide;
    Color fadeColorTop;

    Color mixedColorTop;
    Color mixedColorSide;
    Color oldSideColor;
    Color oldTopColor;
    List<Color> sideColors = new List<Color>();
    List<Color> topColors = new List<Color>();

    float fadeT;
    float fadeSpeed = 1.25f;

    LiquidContainer container;

    // Start is called before the first frame update
    void Start()
    {
        container = GetComponent<LiquidContainer>();

        if (liquidObj == null)
        {
            Debug.LogWarning("Trying to find Liquid in child");
            liquidObj = transform.Find("Liquid").gameObject;
        }

        if (!liquidObj.TryGetComponent(out liquid))
            Debug.LogError("Could not find liquid effect on " + gameObject.name + "'s liquidobject", liquidObj);

        //mat = liquidObj.GetComponent<Renderer>().material;

        targetAmount = liquid.GetLiquid();
        fillAmount = targetAmount;

        //Debug.Log(targetAmount);
        //Debug.Log(mat.GetFloat("_Fill"));
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //fillAmount = mat.GetFloat("_Fill");

    //    if (fillAmount < targetAmount)
    //    {
    //        fillAmount += 0.075f * Time.deltaTime;
    //        mat.SetFloat("_Fill", fillAmount);

    //        Debug.Log("Setting fill to " + fillAmount);
    //    }
    //}

    void ChangeColor()
    {
        if (liquid.GetSideColor() == mixedColorSide && liquid.GetTopColor() == mixedColorTop)
            return;


        liquid.SetSideColor(Color.Lerp(oldSideColor, mixedColorSide, fadeT));
        liquid.SetTopColor(Color.Lerp(oldTopColor, mixedColorTop, fadeT));
        fadeT += fadeSpeed * Time.deltaTime;

    }


    public void AddColors(Color newTopColor, Color newSideColor)
    {
        bool hasColor = false;

        #region topColor
        //If color is already mixed in return
        foreach (var item in topColors)
        {
            if (item == newTopColor)
            {
                hasColor = true;
                break;
            }
        }

        if (!hasColor)
        {
            if (topColors.Count == 0)
                oldSideColor = newTopColor;


            topColors.Add(newTopColor);
            mixedColorTop = PotionColors.CombineColors(topColors.ToArray());
            oldTopColor = liquid.GetTopColor();
            fadeT = 0;
        }
        #endregion


        #region sideColor
        hasColor = false;

        //check if it already has color
        foreach (var item in sideColors)
        {
            if (item == newSideColor)
            {
                hasColor = true;
                break;
            }
        }

        if (!hasColor)
        {
            if (sideColors.Count == 0)
                oldSideColor = newSideColor;

            sideColors.Add(newSideColor);
            mixedColorSide = PotionColors.CombineColors(sideColors.ToArray());
            oldSideColor = liquid.GetSideColor();
            fadeT = 0;
        }
        #endregion

        if (sideColors.Count == 1 && topColors.Count == 1)
        {
            liquid.SetSideColor(mixedColorSide);
            liquid.SetTopColor(mixedColorTop);
        }

    }

    void AddAttributes(GameObject fromObject, float volume)
    {
        IAttribute[] attributes = fromObject.GetComponents<IAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            IAttribute attribute = attributes[i];

            Debug.Log("Adding " + volume + " of " + attribute.GetName() + " to " + transform.name + " from " + fromObject.name);
            attributes[i].AddToOther(transform, volume);
        }
    }

    public void RecieveLiquid(IAttribute attribute, float volume)
    {
        CheckOverflow(volume, out volume);

        PotionColor colors = PotionColors.GetColor(attribute);

        AddColors(colors.topColor, colors.sideColor);
        ChangeColor();

        attribute.AddToOther(transform, volume);

        float currentFill = liquid.GetLiquid();
        currentFill = currentFill > 0 ? currentFill : 0;
        currentFill += volume;
        liquid.SetLiquid(currentFill);
    }

    public void RecieveLiquid(GameObject fromObject, float volume)
    {
        //Debug.Log(gameObject.name + " Recieved " + volume + " from " + fromObject.name);

        CheckOverflow(volume, out volume);

        LiquidContainer fromContainer = fromObject.GetComponent<LiquidContainer>();

        AddColors(fromContainer.GetTopColor(), fromContainer.GetSideColor());
        ChangeColor();
        AddAttributes(fromObject, volume);

        float currentFill = liquid.GetLiquid();
        currentFill = currentFill > 0 ? currentFill : 0;
        currentFill += volume;
        liquid.SetLiquid(currentFill);
    }

    public IEnumerator Couroutine_AddFromDispenser(IAttribute attribute, float liquidLost, float delay)
    {
        incomingLiquid += liquidLost;
        yield return new WaitForSeconds(delay);

        RecieveLiquid(attribute, liquidLost);
        incomingLiquid -= liquidLost;
    }

    public IEnumerator Couroutine_TransferLiquid(GameObject fromObject, float liquidLost, float delay)
    {
        incomingLiquid += liquidLost;
        yield return new WaitForSeconds(delay);

        Debug.Log("Transferred liquid to: " + gameObject.name);
        RecieveLiquid(fromObject, liquidLost);
        incomingLiquid -= liquidLost;
    }

    public float GetVolume()
    {
        float volume = liquid.GetLiquid() + incomingLiquid;
        return volume;
    }

    bool CheckOverflow(float volume, out float adjustedVolume)
    {
        float fill = liquid.GetLiquid();

        if (fill + volume < 1)
        {
            adjustedVolume = volume;
            return false;
        }

        adjustedVolume = 1 - fill;
        return true;
    }
}
