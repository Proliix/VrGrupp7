using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Bouncy")]
public class Bouncy : MonoBehaviour, IScannable, IAttribute
{
    public float bouncyness = 1f;
    Collider[] m_colliders;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        m_colliders = GetComponents<Collider>();

        SetBouncyness(bouncyness);
    }

    void SetBouncyness(float bouncy)
    {
        bouncyness = bouncy;

        for (int i = 0; i < m_colliders.Length; i++)
        {
            PhysicMaterial material = new PhysicMaterial();
            material.bounciness = bouncy;
            m_colliders[i].material = material;
        }
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
