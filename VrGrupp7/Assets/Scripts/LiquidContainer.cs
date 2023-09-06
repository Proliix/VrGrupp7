using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidContainer : MonoBehaviour
{
    [SerializeField] GameObject liquidObject;
    [SerializeField] ParticleSystem particles;
    [SerializeField] AnimationCurve flowRate;

    [Header("Tuneing")]
    [SerializeField] float minimumFillAmount = 0;
    [SerializeField] float emptySpeed = 0.1f;
    [SerializeField] float bigPourMultiplier = 0.75f;

    bool hasParticles;
    float angle;
    float fillAmount;
    Vector3 wobblePos;
    Material mat;


    float forceEmptyAmount = -10;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule mainModule = particles.main;
        mat = liquidObject.GetComponent<MeshRenderer>().material;
        mainModule.startColor = mat.GetColor("_SideColor");
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

    public Color GetSideColor()
    {
        return mat.GetColor("_SideColor");
    }

    public Color GetTopColor()
    {
        return mat.GetColor("_TopColor");
    }

}
