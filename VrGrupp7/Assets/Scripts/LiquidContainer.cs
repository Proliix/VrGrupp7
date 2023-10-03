using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LiquidContainer : MonoBehaviour
{
    [SerializeField] GameObject liquidObject;

    [SerializeField] AnimationCurve flowRate;
    [SerializeField] float fillSpeed = 0.3f;

    [Header("Tuneing")]
    [SerializeField] float minimumFillAmount = 0;
    [SerializeField] float emptySpeed = 0.1f;
    [SerializeField] float bigPourMultiplier = 0.75f;

    [SerializeField] bool isPouring;


    [Header("Debug")]
    [SerializeField] bool debugAngle;

    float angle;
    float fillAmount;
    Vector3 wobblePos;
    Material mat;
    LiquidEffect liquid;
    bool hasCork;
    PourLiquid pourLiquid;

    // Start is called before the first frame update
    void Start()
    {
        mat = liquidObject.GetComponent<MeshRenderer>().material;

        if (!liquidObject.TryGetComponent(out liquid))
            Debug.LogError("Could not find liquid effect on " + gameObject.name + "'s liquidobject", liquidObject);

        if (liquid.GetLiquid() <= 0)
        {
            liquid.EmptyLiquid();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hasCork)
        {
            if (isPouring)
                StopPour();

            return;
        }

        if (liquid == null)
        {
            Debug.LogError("<color=red><b>Error: </b></color>" + gameObject.name + " DOES NOT HAVE A LIQUIDEFFECT. THIS OBJECT WILL NOT WORK. ADD A LIQUID EFFECT TO ITS LIQUID OBJECT", this);
            return;
        }

        fillAmount = liquid.GetLiquid();

        if (fillAmount >= minimumFillAmount)
        {
            //finds wooble pos
            wobblePos = new Vector3(mat.GetFloat("_WobbleX"), 0, mat.GetFloat("_WobbleZ"));

            //find angle for it to start to remove liquid
            angle = flowRate.Evaluate(fillAmount);

            if (debugAngle)
                Debug.Log("Angle: " + Vector3.Angle(transform.up, Vector3.up));

            //check if it is tilted enough for it to spill then start to remove liquid and check if the wobble would make it spill
            if (Vector3.Angle(transform.up + wobblePos, Vector3.up) >= angle)
            {

                float tilt = Vector3.Angle(transform.up, Vector3.up) / angle;
                //Debug.Log(tilt);
                float liquidLost = (emptySpeed * (tilt * bigPourMultiplier) * Time.deltaTime);

                //Debug.Log("Pouring: " + isPouring + " - Lost: " + liquidLost);

                liquid.SetLiquid(fillAmount - liquidLost);
                if (!isPouring)
                {
                    StartPour();
                }
                if (pourLiquid != null)
                {
                    pourLiquid.UpdateLiquidLost(liquidLost);
                    pourLiquid.SetPourStrength(tilt);
                }

                //isPouring = true;
            }
            else if (isPouring)
            {
                Debug.Log(transform.name + " is not tilted enough");
                StopPour();
            }
        }

        if (fillAmount <= minimumFillAmount && !liquid.GetIsEmpty())
        {
            if (isPouring)
            {
                Debug.Log(transform.name + " is below min fill");
                StopPour();
            }

            Empty();
        }
    }

    void StartPour()
    {
        if (TryGetComponent(out pourLiquid))
        {
            isPouring = true;
            pourLiquid.Pour(liquid.GetSideColor());
            Debug.Log("LiquidContainer: Starting Pour");
        }
    }

    public void ForceStopPour()
    {
        if (isPouring)
            StopPour();
    }

    void StopPour()
    {
        if (TryGetComponent(out pourLiquid))
        {
            isPouring = false;
            pourLiquid.Stop();
            Debug.Log("LiquidContainer: Stopping Pour");
        }
    }

    void Empty()
    {
        liquid.EmptyLiquid();
        RemoveAllAtributes();
    }


    public void AddLiquid()
    {
        if (hasCork)
            return;

        if (Vector3.Angle(transform.up, Vector3.up) < 20f)
        {
            if (fillAmount < 0)
                fillAmount = 0;

            if (fillAmount < 1)
            {
                liquid.SetLiquid(fillAmount + (fillSpeed * Time.deltaTime));
            }
            else
                liquid.SetLiquid(1);
        }
    }
    void RemoveAllAtributes()
    {
        IAttribute[] attributes = GetComponents<IAttribute>();
        for (int i = attributes.Length - 1; i >= 0; i--)
        {
            Destroy(attributes[i] as Component);
        }
    }

    public void SetHasCork(bool newHasCork)
    {
        hasCork = newHasCork;
    }

    public Color GetSideColor()
    {
        return liquid.GetSideColor();
    }

    public Color GetTopColor()
    {
        return liquid.GetTopColor();
    }

    void AddAttributes(GameObject other, float volume)
    {
        IAttribute[] attributes = other.GetComponents<IAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].AddToOther(transform, volume);
        }

    }

    public float GetLiquidVolume()
    {
        if (mat == null)
            mat = liquidObject.GetComponent<MeshRenderer>().material;

        return Mathf.Clamp01(liquid.GetLiquid());
    }
}
