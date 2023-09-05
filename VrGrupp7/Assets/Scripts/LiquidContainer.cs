using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidContainer : MonoBehaviour
{
    [SerializeField] GameObject liquidObject;
    [SerializeField] float filledTiltRange, minTiltRange;
    [SerializeField] float maxLiquid, minLiquid;


    float angle;
    float fillAmount;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = liquidObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        fillAmount = mat.GetFloat("_Fill");
        //find angle for it to start to remove liquid
        angle = Mathf.Lerp(minTiltRange, filledTiltRange, fillAmount);

        //Debug.Log("Angle to spill: " + angle + " | current angle: " + Vector3.Angle(transform.up, Vector3.up));

        //check if it is tilted enough for it to spill then start to remove liquid
        if (Vector3.Angle(transform.up, Vector3.up) >= angle)
            mat.SetFloat("_Fill", fillAmount - (0.1f * (angle / Vector3.Angle(transform.up, Vector3.up))) * Time.deltaTime);
    }
}
