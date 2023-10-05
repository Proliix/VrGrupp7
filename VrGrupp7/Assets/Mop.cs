using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CanHaveAttributes attributes))
        {
            attributes.RemoveAllAttributes();
        }
    }
}
