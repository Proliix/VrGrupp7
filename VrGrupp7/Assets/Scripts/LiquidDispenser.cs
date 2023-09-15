using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum DispensingType
{
    Gravity,
    Bouncy,
    Cloning
}

[RequireComponent(typeof(Lever))]
public class LiquidDispenser : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    [SerializeField] Vector3 fillPos;
    [SerializeField] Vector3 fillhalfExstents;
    public DispensingType type;

    Lever lever;

    IAttribute currentAttribute;

    bool isActive;
    Vector3 startPos;
    Color currentColor = Color.red;
    LiquidContainer container;

    void Start()
    {
        lever = GetComponent<Lever>();
        lever.onEnable.AddListener(StartDispensing);
        lever.onDisable.AddListener(StopDispensing);
        startPos = transform.position;
    }

    private void Update()
    {
        if (isActive)
        {
            DispensingUpdate();
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
    }

    void StopDispensing()
    {
        isActive = false;
        particle.Stop();
    }

    IAttribute GetAttribute(DispensingType newType)
    {
        if (container != null)
        {

            type = newType;
            switch (type)
            {
                case DispensingType.Gravity:
                    CustomGravity newGravity;
                    if (container.gameObject.TryGetComponent(out newGravity) == false)
                    {
                        newGravity = container.gameObject.AddComponent<CustomGravity>();
                        newGravity.gravityScale = 0;
                    }
                    currentColor = PotionColors.GravitySide;
                    container.AddColors(PotionColors.GravityTop, PotionColors.GravitySide);
                    currentAttribute = newGravity;
                    break;
                case DispensingType.Bouncy:
                    Bouncy newBouncy;
                    if (container.gameObject.TryGetComponent(out newBouncy) == false)
                    {
                        newBouncy = container.gameObject.AddComponent<Bouncy>();
                    }
                    currentColor = PotionColors.BouncySide;
                    container.AddColors(PotionColors.BouncyTop, PotionColors.BouncySide);
                    currentAttribute = newBouncy;
                    break;
                case DispensingType.Cloning:
                    Duplicator newDuplicator;
                    if (container.gameObject.TryGetComponent(out newDuplicator) == false)
                    {
                        newDuplicator = container.gameObject.AddComponent<Duplicator>();
                    }
                    currentColor = PotionColors.CloningSide;
                    container.AddColors(PotionColors.CloningTop, PotionColors.CloningSide);
                    currentAttribute = newDuplicator;
                    break;
                default:
                    Debug.LogError("BECAME DEFAULT CASE");
                    break;
            }
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.startColor = currentColor;
        }
        return currentAttribute;
    }

    void UpdateAtribute()
    {
        container = FindContainerInBounds();
        GetAttribute(type);

        if (container == null || currentAttribute == null)
            return;

        currentAttribute.AddToOther(container.transform);
    }



    private void OnDrawGizmosSelected()
    {
        if (startPos != Vector3.zero)
            Gizmos.DrawWireCube(startPos + fillPos, fillhalfExstents);
        else
            Gizmos.DrawWireCube(transform.position + fillPos, fillhalfExstents);
    }

}
