using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttribute : MonoBehaviour, IScannable, IAttribute
{
    private LiquidContainer liquidContainer;

    public float potency
    {
        get { return GetPotency(); }
    }

    public float mass = 0;

    public void AddMass(float addMass)
    {
        mass += addMass;

        UpdateStats();
    }
    public void AddMass(float otherMass, float volume)
    {
        mass += otherMass * volume;

        UpdateStats();
    }

    public float LoseMass(float volume)
    {
        float lostMass = (potency*100) * volume;
        mass -= lostMass;

        UpdateStats();

        return lostMass;
    }

    public void TransferMass(BaseAttribute other, float volume)
    {
        float lostMass = other.LoseMass(volume);
        AddMass(lostMass);
    }

    public float GetPotency()
    {
        float volume = 1;
        if (liquidContainer == null)
        {
            if (TryGetComponent(out liquidContainer))
            {
                volume = liquidContainer.GetLiquidVolume();
            }
        }
        else
            volume = liquidContainer.GetLiquidVolume();

        if (volume == 0)
            return 0;

        float potency = ((mass) * 0.01f) / volume;

        //if (potency > 1 || potency < 0)
        //    Debug.Log(transform.name + " has this potency" + potency + " from mass: " + mass + " and vol: " + volume);

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

        //otherComponent.addMass(mass, volume);

        otherComponent.TransferMass(this, volume);

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
