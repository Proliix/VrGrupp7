using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_rocks_lever_down : MonoBehaviour
{
    public GameObject prefab_rock;
    public GameObject gameobject_spawn_pos;
    public Vector3 spawn_pos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collision_spawn_rocks")
        {
        spawn_pos = gameobject_spawn_pos.transform.position;
        Instantiate(prefab_rock, spawn_pos, Quaternion.identity);
        }
    }
}

