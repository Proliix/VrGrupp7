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
    //How mixed the mass is
    [Range(0, 1f)] 
    public float mixed01 = 0;

    private float maxPotency = 1;


    private void Awake()
    {
        if(GetComponent<LiquidContainer>() == null && GetComponent<CanHaveAttributes>() == null)
        {
            enabled = false;
        }

        if (TryGetComponent(out Shake shake))
        {
            shake.onShake.AddListener(Shake);
        }
    }

    public void AddMass(float addMass)
    {
        mass += addMass;
        mixed01 = mixed01 * (mass * 0.01f);

        if(enabled)
            UpdateStats();
    }
    public void AddMass(float otherMass, float volume)
    {
        mass += otherMass * volume;

        if(enabled)
            UpdateStats();
    }

    public float LoseMass(float volume)
    {
        float lostMass = (GetConcentration() * 100) * volume;
        mass -= lostMass;

        if(enabled)
            UpdateStats();

        return lostMass;
    }

    public void TransferMass(BaseAttribute other, float volume)
    {
        float otherMixed = other.mixed01;

        float lostMass = other.LoseMass(volume);

        float otherMixedMass = lostMass * otherMixed;

        float mixedMass = mass * mixed01;

        float combinedMixedMass = mixedMass + otherMixedMass;

        AddMass(lostMass);

        mixed01 = combinedMixedMass / mass;
    }

    public float GetPotency()
    {
        float volume = GetVolume();

        if (volume == 0)
            return 0;

        float potency = ((mass*mixed01) * 0.01f) / volume;

        //if (potency > 1 || potency < 0)
        //    Debug.Log(transform.name + " has this potency" + potency + " from mass: " + mass + " and vol: " + volume);

        return potency;
    }

    float GetVolume()
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

        return volume;
    }

    float GetConcentration()
    {
        float volume = GetVolume();

        if (volume == 0)
            return 0;

        float concentration = (mass * 0.01f) / volume;

        return concentration;
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

        otherComponent.TransferMass(this, volume);

        Debug.Log("Adding " + type.Name + " to " + other.name);
    }

    public BaseAttribute AddToOther(Transform other)
    {
        var type = GetType();
        BaseAttribute otherComponent = (BaseAttribute)other.GetComponent(type);

        if (otherComponent == null)
        {
            otherComponent = (BaseAttribute)other.gameObject.AddComponent(type);
            otherComponent.OnComponentAdd(this);
        }

        return otherComponent;
    }

    private void Shake(float shakeForce)
    {
        TryMix(shakeForce);
    }

    private void TryMix(float addedPercentage)
    {
        float newMixedvalue = mixed01 + addedPercentage;
        float volume = GetVolume();

        if (volume == 0)
        {
            mixed01 = 0;
            return; 
        }

        float newPotency = ((mass * 0.01f) * newMixedvalue) / volume;

        // Math to calculate max mixed value
        // maxP = ((mass * maxMixedValue * 0.01 / volume)
        // maxP*volume = (mass * 0.01 * maxMixedValue)
        // maxP*volume / (mass * 0.01) = maxMixedValue

        //Limit potency to not exceed maxPotency
        if (newPotency > maxPotency)
        {
            newMixedvalue = (maxPotency * volume) / (mass * 0.01f);
        }

        mixed01 = Mathf.Clamp01(newMixedvalue);

        //mixed01 = Mathf.Clamp01(mixed01 += shakeForce);
        Debug.Log(name + " is " + Mathf.Round(mixed01 * 100) + "% mixed");

        if(enabled)
            UpdateStats();
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
