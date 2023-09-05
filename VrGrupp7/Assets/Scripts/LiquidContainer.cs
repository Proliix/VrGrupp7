using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidContainer : MonoBehaviour
{
    [SerializeField] GameObject liquidObject;
    [SerializeField] ParticleSystem particles;
    [SerializeField] float filledTiltRange, minTiltRange;
    [SerializeField] float maxLiquid, minLiquid;

    bool hasParticles;
    float angle;
    float fillAmount;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = liquidObject.GetComponent<MeshRenderer>().material;
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


    // Update is called once per frame
    void Update()
    {
        fillAmount = mat.GetFloat("_Fill");

        if (fillAmount > 0)
        {
            //find angle for it to start to remove liquid
            angle = Mathf.Lerp(minTiltRange, filledTiltRange, fillAmount);

            //Debug.Log("Angle to spill: " + angle + " | current angle: " + Vector3.Angle(transform.up, Vector3.up));

            //check if it is tilted enough for it to spill then start to remove liquid
            if (Vector3.Angle(transform.up, Vector3.up) >= angle)
            {
                mat.SetFloat("_Fill", fillAmount - (0.1f * (angle / Vector3.Angle(transform.up, Vector3.up))) * Time.deltaTime);
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
        else if (hasParticles)
        {
            StopParticles();
        }
    }
}
