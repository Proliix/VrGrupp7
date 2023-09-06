using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_rocks_lever_down : MonoBehaviour
{
    //Script description: When player pulls down the lever and touch invisible trigger box
    //there will spawn an Rock on position of the gamobject called Spawn Rocks.
    public GameObject prefab_rock;
    public GameObject gameobject_spawn_rocks;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collision_spawn_rocks")
        {
        Instantiate(prefab_rock, gameobject_spawn_rocks.transform.position, Quaternion.identity);
        }
    }
}

