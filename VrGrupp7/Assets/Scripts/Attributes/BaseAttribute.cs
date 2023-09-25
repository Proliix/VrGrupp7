using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttribute : MonoBehaviour, IScannable, IAttribute
{
    public float potency = 0;

    public void AddPotency(float addPotency)
    {
        potency += addPotency;

        UpdateStats();
    }
    public void AddPotency(float otherPotency, float volume)
    {
        potency += otherPotency * volume;

        UpdateStats();
    }

    public float LosePotency(float volume)
    {
        float lostPotency = potency * volume;
        potency -= lostPotency;

        UpdateStats();

        return lostPotency;
    }

    public void TransferPotency(BaseAttribute other, float volume)
    {
        float lostPotency = other.LosePotency(volume);
        AddPotency(lostPotency);
    }

    public float GetPotency()
    {
        return potency;
    }

    public void AddToOther(Transform other, float volume)
    {
        var type = GetType();
        BaseAttribute otherComponent = (BaseAttribute)other.GetComponent(type);

        if(otherComponent == null)
        {
            otherComponent = (BaseAttribute)other.gameObject.AddComponent(type);
            otherComponent.OnComponentAdd(this);
        }

        //otherComponent = otherComponent == null ? (BaseAttribute)other.gameObject.AddComponent(type) : otherComponent;

        otherComponent.AddPotency(potency, volume);

        Debug.Log("Adding " + type.Name + " to " + other.name);
    }

    public virtual void OnComponentAdd(BaseAttribute originalAttribute)
    {

    }

    public virtual void UpdateStats()
    {

    }

    public abstract string GetName();

    public abstract string GetScanInformation();
}
