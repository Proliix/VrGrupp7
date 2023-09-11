using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class CanHaveAttributes : MonoBehaviour
{
    void AddAttributes(GameObject other)
    {
        IAttribute[] attributes = other.GetComponents<IAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].AddToOther(transform);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();

        if (container != null)
        {
            AddAttributes(other.transform.parent.gameObject);
        }
    }
}
