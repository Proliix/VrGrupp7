using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Bouncy")]
public class Bouncy : BaseAttribute
{
    Collider[] m_colliders;
    PhysicMaterial material;

    void OnEnable()
    {
        Init();
        SetBouncyness(potency);
    }

    void Init()
    {
        m_colliders = GetComponents<Collider>();
        material = new PhysicMaterial();

        for (int i = 0; i < m_colliders.Length; i++)
        {
            m_colliders[i].material = material;
        }
    }

    void SetBouncyness(float bouncy)
    {
        //potency = bouncy;
        material.bounciness = Mathf.Clamp01(bouncy);

        //for (int i = 0; i < m_colliders.Length; i++)
        //{
        //    m_colliders[i].material = material;
        //}
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
