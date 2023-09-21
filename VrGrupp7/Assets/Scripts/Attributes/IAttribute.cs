using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttribute
{
    void AddToOther(Transform other, float volume);
    string GetName();
    float GetPotency();
}
