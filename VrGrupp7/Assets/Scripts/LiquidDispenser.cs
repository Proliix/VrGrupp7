using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum PotionType
{
    Gravity,
    Bouncy,
    Cloning,
    Transparancy,
    Explosive
}

[RequireComponent(typeof(Lever))]
public class LiquidDispenser : MonoBehaviour
{
    [SerializeField] PourLiquid pourLiquid;
    [SerializeField] ParticleSystem particle;
    [SerializeField] Vector3 fillPos;
    [SerializeField] Vector3 fillhalfExstents;
    [SerializeField] Material transparacyMat;
    [SerializeField] GameObject explosion;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip stopSound;
    [SerializeField] AudioSource audioSource;
    public PotionType type;

    Lever lever;

    public IAttribute currentAttribute;


    bool isActive;
    Vector3 startPos;
    Color currentColor = Color.red;
    LiquidContainer container;
    [SerializeField] float liquidPerSecond = 0.5f;

    void Start()
    {
        lever = GetComponent<Lever>();
        //lever.onEnable.AddListener(StartDispensing);
        //lever.onDisable.AddListener(StopDispensing);

        lever.onEnable.AddListener(StartPour);
        lever.onDisable.AddListener(StopPour);


        startPos = transform.position;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
    }

    private void Update()
    {
        if (isActive)
        {
            //DispensingUpdate();
            UpdatePour();
        }
    }

    void DispensingUpdate()
    {

        container = FindContainerInBounds();

        UpdateAtribute();
        container?.AddLiquid();

    }

    LiquidContainer FindContainerInBounds()
    {
        Collider[] hitColliders = Physics.OverlapBox(startPos + fillPos, fillhalfExstents);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            LiquidContainer newContainer;
            if (hitColliders[i].gameObject.TryGetComponent(out newContainer) == false)
                continue;

            return newContainer;
        }

        return null;
    }

    void StartDispensing()
    {
        container = FindContainerInBounds();
        GetAttribute(type);
        DispensingUpdate();
        isActive = true;
        ParticleSystem.MainModule mainModule = particle.main;
        mainModule.startColor = currentColor;
        particle.Play();
        audioSource.PlayOneShot(startSound);
        Invoke(nameof(StartLoopSound), startSound.length - 0.01f);
    }

    void StartLoopSound()
    {
        if (isActive)
            audioSource.Play();
    }

    void StopDispensing()
    {
        isActive = false;
        particle.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(stopSound);
    }

    IAttribute GetAttribute(PotionType newType)
    {
        type = newType;
        switch (type)
        {
            case PotionType.Gravity:
                CustomGravity gravity = GameObject.FindWithTag("GameController").GetComponent<CustomGravity>();

                if (gravity == null)
                    Debug.LogError("DID NOT GET CUSTOMGRAVITY FROM GAMECONTROLLER", this);

                currentColor = PotionColors.GravitySide;
                //container.AddColors(PotionColors.GravityTop, PotionColors.GravitySide);
                currentAttribute = gravity;
                break;

            case PotionType.Bouncy:
                Bouncy bouncy = GameObject.FindWithTag("GameController").GetComponent<Bouncy>();

                if (bouncy == null)
                    Debug.LogError("DID NOT GET BOUNCY FROM GAMECONTROLLER", this);

                currentColor = PotionColors.BouncySide;
                //container.AddColors(PotionColors.BouncyTop, PotionColors.BouncySide);
                currentAttribute = bouncy;
                break;
            case PotionType.Cloning:
                Duplicator duplicator = GameObject.FindWithTag("GameController").GetComponent<Duplicator>();

                if (duplicator == null)
                    Debug.LogError("DID NOT GET DUPLICATOR FROM GAMECONTROLLER", this);

                currentColor = PotionColors.CloningSide;
                //container.AddColors(PotionColors.CloningTop, PotionColors.CloningSide);
                currentAttribute = duplicator;
                break;

            case PotionType.Transparancy:
                Transparency transparency = GameObject.FindWithTag("GameController").GetComponent<Transparency>();

                if (transparency == null)
                    Debug.LogError("DID NOT GET TRANSPARANCY FROM GAMECONTROLLER", this);

                currentColor = PotionColors.TransparencySide;
                //container.AddColors(PotionColors.TransparencyTop, PotionColors.TransparencySide);
                currentAttribute = transparency;
                break;
            case PotionType.Explosive:
                Explosive explosive = GameObject.FindWithTag("GameController").GetComponent<Explosive>();

                if (explosive == null)
                    Debug.LogError("DID NOT GET EXPLOSIVE FROM GAMECONTROLLER",this);
                    
                currentColor = PotionColors.ExplosiveSide;
                //container.AddColors(PotionColors.ExplosiveTop, PotionColors.ExplosiveSide);
                currentAttribute = explosive;
                break;

            default:
                Debug.LogError("BECAME DEFAULT CASE");
                break;
        }
        //ParticleSystem.MainModule mainModule = particle.main;
        //mainModule.startColor = currentColor;
        return currentAttribute;
    }

    void UpdateAtribute()
    {
        container = FindContainerInBounds();
        GetAttribute(type);

        if (container == null || currentAttribute == null)
            return;

        currentAttribute.AddToOther(container.transform, 0.01f);
    }

    void StartPour()
    {
        isActive = true;
        GetAttribute(type);
        pourLiquid.Pour(currentColor);
    }

    void UpdatePour()
    {
        pourLiquid.UpdateLiquidLost(liquidPerSecond * Time.deltaTime);
    }

    void StopPour()
    {
        isActive = false;
        pourLiquid.Stop();
    }



    private void OnDrawGizmosSelected()
    {
        if (startPos != Vector3.zero)
            Gizmos.DrawWireCube(startPos + fillPos, fillhalfExstents * 2);
        else
            Gizmos.DrawWireCube(transform.position + fillPos, fillhalfExstents * 2);
    }

}
