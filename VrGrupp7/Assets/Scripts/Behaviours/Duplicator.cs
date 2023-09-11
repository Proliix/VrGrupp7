using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour, IAttribute
{
    [Range(0, 1)]
    public float progress;
    

    void UpdateProgress()
    {

    }

    public void AddToOther(Transform other)
    {
        throw new System.NotImplementedException();
    }


}
