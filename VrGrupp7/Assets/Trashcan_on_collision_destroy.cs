using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan_on_collision_destroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent(out Respawnable respawnable))
        {
            respawnable.Respawn();
            return;
        }

        if (other.attachedRigidbody != null)
        {
            Destroy(other.gameObject);
        }
    }
}
