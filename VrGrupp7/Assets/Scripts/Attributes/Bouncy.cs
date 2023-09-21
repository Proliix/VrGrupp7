using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Bouncy")]
public class Bouncy : BaseAttribute
{
    Collider[] m_colliders;

    void OnEnable()
    {
        m_colliders = GetComponents<Collider>();

        SetBouncyness(potency);
    }

    void SetBouncyness(float bouncy)
    {
        potency = bouncy;

        for (int i = 0; i < m_colliders.Length; i++)
        {
            PhysicMaterial material = new PhysicMaterial();
            material.bounciness = bouncy;
            m_colliders[i].material = material;
        }
    }

    public override void UpdateStats()
    {
        SetBouncyness(potency);
    }

    public override string GetScanInformation()
    {
        return "Bouncy";
    }

    public override string GetName()
    {
        return "Bouncy";
    }
}
