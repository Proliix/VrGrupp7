using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bouncy : MonoBehaviour, IScannable,IAttribute
{
    public float bouncyness = 1f;
    Collider m_collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        m_collider = GetComponent<Collider>();
        SetBouncyness(bouncyness);
    }

    void SetBouncyness(float bouncy)
    {
        bouncyness = bouncy;

        PhysicMaterial material = new PhysicMaterial();
        material.bounciness = bouncy;
        m_collider.material = material;
    }

    public string GetScanInformation()
    {
        return "Bouncy";
    }

    public void AddToOther(Transform other)
    {
        Bouncy otherBouncy = other.GetComponent<Bouncy>();
        otherBouncy = otherBouncy == null ? other.gameObject.AddComponent<Bouncy>() : otherBouncy;
        otherBouncy.bouncyness = bouncyness;
    }
}
