using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("**Attributes**/Duplicator")]
public class Duplicator : BaseAttribute
{
    //public float potency;
    float cloneSpeed = 0.3f;
    bool isDuping = false;


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
        if (potency >= 1 && !isDuping)
        {
            isDuping = true;
            Duplicate();
        }
    }

    void Duplicate()
    {
        GameObject DupedObj = Instantiate(gameObject, transform.position + Vector3.up * 0.25f, transform.rotation);

        Destroy(DupedObj.GetComponent<Duplicator>());
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
