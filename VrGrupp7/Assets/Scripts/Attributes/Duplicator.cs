using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("**Attributes**/Duplicator")]
public class Duplicator : BaseAttribute
{
    float massRequired = 50;

    //public void AddToOther(Transform other, float volume)
    //{
    //    Duplicator otherDuplicator = other.GetComponent<Duplicator>();
    //    otherDuplicator = otherDuplicator == null ? other.gameObject.AddComponent<Duplicator>() : otherDuplicator;

    //    otherDuplicator.UpdateProgress();

    //}

    //public void UpdateProgress()
    //{
    //    potency += cloneSpeed * Time.deltaTime;

    //    if (potency >= 1 && !isDuping)
    //    {
    //        isDuping = true;
    //        Duplicate();
    //    }
    //}

    public override void UpdateStats()
    {
        float max = Mathf.Max(mixed01, potency);

        if (mass > massRequired && max > 0.9f)
        {
            Duplicate();
        }
    }

    void Duplicate()
    {
        mixed01 = 0;
        TryUpdateColor();

        float height = 0.25f;

        if (TryGetComponent(out Renderer renderer))
        {
            height = renderer.bounds.size.y;
        }

        GameObject DupedObj = Instantiate(gameObject, transform.position + Vector3.up * height, transform.rotation);

        var clonedDuplicator = DupedObj.GetComponent<Duplicator>();

        clonedDuplicator.mixed01 = 0;

        //if(TryGetComponent(out Shake shake))
        //{
        //    shake.onShake = new UnityEngine.Events.UnityEvent<float>();
        //}

        if(clonedDuplicator.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;

            rigidbody.useGravity = clonedDuplicator.GetComponent<CustomGravity>() == null;
        }
        

        Destroy(clonedDuplicator, 0.1f);
        Destroy(this);
    }

    public override string GetScanInformation()
    {
        return "Cloning: " + (potency * 100).ToString("F0") + "%";
    }

    //public float GetPotency()
    //{
    //    return potency;
    //}

    public override string GetName()
    {
        return "Cloning";
    }
}
