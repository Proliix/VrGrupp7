using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidCatcher : MonoBehaviour
{
    [SerializeField] GameObject liquidObj;
    [SerializeField] float liquidAddAmount = 0.1f;
    [SerializeField] float speed = 0.01f;

    Material mat;
    float targetAmount;
    float fillAmount;

    Color fadeColorSide;
    Color fadeColorTop;
    Color oldSide;
    Color oldTop;
    float fadeT;
    float fadeSpeed = 0.75f;

    LiquidContainer lastContainer;

    // Start is called before the first frame update
    void Start()
    {
        mat = liquidObj.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        fillAmount = mat.GetFloat("_Fill");

        if (fillAmount < targetAmount)
        {
            fillAmount += 0.075f * Time.deltaTime;
            mat.SetFloat("_Fill", fillAmount);
        }

        

    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Is here");

        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();

        if (container != null)
        {
            if (lastContainer == null)
            {
                mat.SetColor("_SideColor", container.GetSideColor());
                mat.SetColor("_TopColor", container.GetTopColor());

                fadeColorSide = container.GetSideColor();
                fadeColorTop = container.GetTopColor();

            }
            else if (container != lastContainer)
            {
                fadeT = 0;
                oldSide = mat.GetColor("_SideColor");
                oldTop = mat.GetColor("_TopColor");

                fadeColorSide = mat.GetColor("_SideColor") + container.GetSideColor();
                fadeColorTop = mat.GetColor("_TopColor") + container.GetTopColor();

            }

            if (mat.GetColor("_SideColor") != fadeColorSide)
            {
                mat.SetColor("_SideColor", Color.Lerp(oldSide, fadeColorSide, fadeT));
                mat.SetColor("_TopColor", Color.Lerp(oldTop, fadeColorTop, fadeT));
                fadeT += fadeSpeed * Time.deltaTime;
            }

            targetAmount += liquidAddAmount;

            lastContainer = container;
        }
    }



}
