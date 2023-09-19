using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WantedAttribute
{
    public IAttribute Attribute;
    //FIX A CUSTOM EDITOR AND REMOVE "AttributeName" PLS
    //THIS IS BAD
    public string AttributeName;
    public float potency;
    public bool wantGreater;
}

public class JobManager : MonoBehaviour
{
    public List<WantedAttribute> wantedAtributes;
    [SerializeField] int startBatch = 2;

    IAttribute[] allAttributes;

    // Start is called before the first frame update
    void Start()
    {
        allAttributes = GetComponents<IAttribute>();
        GetNewBatch(startBatch);
    }
    
    [ContextMenu("Get New Attributes")]
    void NewBatchEditor()
    {
        GetNewBatch(startBatch);
    }

    void GetNewBatch(int amount)
    {
        wantedAtributes = new List<WantedAttribute>();
        for (int i = 0; i < amount; i++)
        {
            wantedAtributes.Add(NewWantedAttribute());
        }
    }

    WantedAttribute NewWantedAttribute()
    {
        WantedAttribute wanted = new WantedAttribute();
        int i = Random.Range(0, allAttributes.Length);
        wanted.Attribute = allAttributes[i];
        bool wantGreater = Random.Range(0, 2) > 0;
        wanted.AttributeName = allAttributes[i].GetType().ToString();
        wanted.wantGreater = wantGreater;
        float potency = Random.Range(wantGreater ? 0f : 0.1f, wantGreater ? 0.9f : 1f);
        wanted.potency = Mathf.Round(potency * 100.0f) * 0.01f;
        return wanted;
    }


}
