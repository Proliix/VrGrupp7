using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LiquidContainer : MonoBehaviour
{
    [SerializeField] GameObject liquidObject;
    [SerializeField] ParticleSystem particles;
    [SerializeField] AnimationCurve flowRate;
    [SerializeField] float fillSpeed = 0.3f;

    [Header("Tuneing")]
    [SerializeField] float minimumFillAmount = 0;
    [SerializeField] float emptySpeed = 0.1f;
    [SerializeField] float bigPourMultiplier = 0.75f;

    bool hasParticles;
    float angle;
    float fillAmount;
    Vector3 wobblePos;
    Material mat;

    //COLOR MIXING
    List<Color> sideColors = new List<Color>();
    List<Color> topColors = new List<Color>();
    Color mixedSideColor;
    Color mixedTopColor;
    Color oldSideColor;
    Color oldTopColor;
    float mixedT;
    float mixSpeed = 0.5f;


    float forceEmptyAmount = -10;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule mainModule = particles.main;
        mat = liquidObject.GetComponent<MeshRenderer>().material;
        mainModule.startColor = mat.GetColor("_SideColor");

        AddColors(mat.GetColor("_TopColor"), mat.GetColor("_SideColor"));
    }

    void StartParticles()
    {
        hasParticles = true;
        particles.Play();
    }

    void StopParticles()
    {
        hasParticles = false;
        particles.Stop();
    }

    void ForceEmpty()
    {
        if (mat.GetFloat("_Fill") > 0)
            mat.SetFloat("_Fill", fillAmount - (emptySpeed * 3) * Time.deltaTime);
        else
            mat.SetFloat("_Fill", forceEmptyAmount);

        RemoveAllAtributes();
    }

    // Update is called once per frame
    void Update()
    {
        fillAmount = mat.GetFloat("_Fill");

        if (fillAmount >= minimumFillAmount)
        {
            //finds wooble pos
            wobblePos = new Vector3(mat.GetFloat("_WobbleX"), 0, mat.GetFloat("_WobbleZ"));

            //find angle for it to start to remove liquid
            angle = flowRate.Evaluate(fillAmount);

            //check if it is tilted enough for it to spill then start to remove liquid and check if the wobble would make it spill
            if (Vector3.Angle(transform.up + wobblePos, Vector3.up) >= angle)
            {
                mat.SetFloat("_Fill", fillAmount - (emptySpeed * ((Vector3.Angle(transform.up, Vector3.up) / angle) * bigPourMultiplier)) * Time.deltaTime);
                if (!hasParticles)
                {
                    StartParticles();
                }
            }
            else if (hasParticles)
            {
                StopParticles();
            }
        }
        else if (fillAmount > forceEmptyAmount)
        {
            if (hasParticles)
                StopParticles();

            ForceEmpty();
        }
    }

    public void AddLiquid()
    {
        if (Vector3.Angle(transform.up, Vector3.up) < 20f)
        {
            UpdateColor();

            if (fillAmount < 0)
                fillAmount = 0;

            if (fillAmount < 1)
            {
                mat.SetFloat("_Fill", fillAmount + (fillSpeed * Time.deltaTime));
            }
            else
                mat.SetFloat("_Fill", 1);
        }
    }

    void UpdateColor()
    {
        if (GetSideColor() == mixedSideColor && GetTopColor() == mixedTopColor)
            return;


        mat.SetColor("_SideColor", Color.Lerp(oldSideColor, mixedSideColor, mixedT));
        mat.SetColor("_TopColor", Color.Lerp(oldTopColor, mixedTopColor, mixedT));
        mixedT += mixSpeed * Time.deltaTime;

        ParticleSystem.MainModule mainModule = particles.main;
        mainModule.startColor = mat.GetColor("_SideColor");

    }

    void RemoveAllAtributes()
    {
        IAttribute[] attributes = GetComponents<IAttribute>();
        for (int i = attributes.Length - 1; i >= 0; i--)
        {
            Destroy(attributes[i] as Component);
        }

        ResetColors();
    }

    void ResetColors()
    {
        sideColors = new List<Color>();
        topColors = new List<Color>();
    }

    Color CombineColors(Color[] aColors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Length;
        return result;
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
            mixedTopColor = CombineColors(topColors.ToArray());
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
            mixedSideColor = CombineColors(sideColors.ToArray());
        }
        #endregion

        oldSideColor = GetSideColor();
        oldTopColor = GetTopColor();
    }

    public Color GetSideColor()
    {
        return mat.GetColor("_SideColor");
    }

    public Color GetTopColor()
    {
        return mat.GetColor("_TopColor");
    }

}
