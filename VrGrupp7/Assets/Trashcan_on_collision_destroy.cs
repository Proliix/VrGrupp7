using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan_on_collision_destroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        Destroy(other.gameObject);
    }
}
