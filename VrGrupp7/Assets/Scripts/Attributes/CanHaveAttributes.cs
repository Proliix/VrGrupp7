using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public class CanHaveAttributes : MonoBehaviour
{
    public UnityEvent onValueChanged;

    public void AddAttributes(GameObject other, float volume)
    {
        IAttribute[] attributes = other.GetComponents<IAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].AddToOther(transform, volume);
        }

        onValueChanged.Invoke();
    }

    private void OnParticleCollision(GameObject other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();

        if (container != null)
        {
            AddAttributes(other.transform.parent.gameObject, 0.01f);
        }
    }
}
