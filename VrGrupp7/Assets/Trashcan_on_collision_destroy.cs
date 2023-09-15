using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan_on_collision_destroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent(out CanHaveAttributes _))
        {
            Destroy(other.gameObject);
        }

        if(other.TryGetComponent(out LiquidContainer _))
        {
            Destroy(other.gameObject);
        }
    }
}
