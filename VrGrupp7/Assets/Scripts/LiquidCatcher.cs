using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LiquidContainer))]

public class LiquidCatcher : MonoBehaviour
{
    [SerializeField] GameObject liquidObj;
    [SerializeField] float liquidAddAmount = 0.1f;

    Material mat;
    float targetAmount = -10;
    float fillAmount;

    Color fadeColorSide;
    Color fadeColorTop;

    Color mixedColorTop;
    Color mixedColorSide;
    Color oldSideColor;
    Color oldTopColor;
    List<Color> sideColors = new List<Color>();
    List<Color> topColors = new List<Color>();

    float fadeT;
    float fadeSpeed = 0.75f;

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


        mat = liquidObj.GetComponent<Renderer>().material;

        targetAmount = mat.GetFloat("_Fill");
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
        if (mat.GetColor("_SideColor") == mixedColorSide && mat.GetColor("_TopColor") == mixedColorTop)
            return;


        mat.SetColor("_SideColor", Color.Lerp(oldSideColor, mixedColorSide, fadeT));
        mat.SetColor("_TopColor", Color.Lerp(oldTopColor, mixedColorTop, fadeT));
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
            oldTopColor = mat.GetColor("_TopColor");
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
            oldSideColor = mat.GetColor("_SideColor");
            fadeT = 0;
        }
        #endregion

    }

    void AddAttributes(GameObject other)
    {
        IAttribute[] attributes = other.GetComponents<IAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].AddToOther(transform);
        }
    }

    public void RecieveLiquid(GameObject fromObject, float volume)
    {
        //Debug.Log(gameObject.name + " Recieved " + volume + " from " + fromObject.name);

        LiquidContainer fromContainer = fromObject.GetComponent<LiquidContainer>();

        AddColors(fromContainer.GetTopColor(), fromContainer.GetSideColor());
        ChangeColor();
        AddAttributes(fromObject);

        float currentFill = mat.GetFloat("_Fill");

        currentFill = currentFill > 0 ? currentFill : 0;

        currentFill += volume;

        mat.SetFloat("_Fill", currentFill);
    }

    private void OnParticleCollision(GameObject other)
    {

        if (container != null)
        {
            AddColors(container.GetTopColor(), container.GetSideColor());
            ChangeColor();
            AddAttributes(other.transform.parent.gameObject);

            if (targetAmount < 0)
            {
                targetAmount = 0;
                fillAmount = 0;
                mat.SetFloat("_Fill", fillAmount);
            }

            targetAmount += liquidAddAmount;
        }
    }



}
