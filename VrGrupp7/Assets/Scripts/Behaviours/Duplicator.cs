using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("**Attributes**/Duplicator")]
public class Duplicator : MonoBehaviour, IScannable, IAttribute
{
    [Range(0, 1)]
    public float progress;

    float cloneSpeed = 0.3f;
    bool isDuping = false;
    
    
    public void AddToOther(Transform other)
    {
        Duplicator otherDuplicator = other.GetComponent<Duplicator>();
        otherDuplicator = otherDuplicator == null ? other.gameObject.AddComponent<Duplicator>() : otherDuplicator;

        otherDuplicator.UpdateProgress();

    }

    public void UpdateProgress()
    {
        progress += cloneSpeed * Time.deltaTime;

        if (progress >= 1 && !isDuping)
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

    public string GetScanInformation()
    {
        return "Cloning: " + (progress * 100).ToString("F0") + "%";
    }
}
