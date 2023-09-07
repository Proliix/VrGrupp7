using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_rocks_lever_down : MonoBehaviour
{
    //Script description: When player pulls down the lever and touch invisible trigger box
    //there will spawn an Rock on position of the gamobject called Spawn Rocks.
    public GameObject prefab_rock;
    public GameObject gameobject_spawn_rocks;
    public AudioSource audioSource;
    public AudioClip lever_sound;
    int max_rock_in_scene = 30;
    int current_rocks_in_scene = 0;
    float timer_takes_to_spawn_rock = 4.0f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collision_spawn_rocks" && max_rock_in_scene > current_rocks_in_scene)
        {
            audioSource.PlayOneShot(lever_sound);
            Invoke("Instantiate_Rock", timer_takes_to_spawn_rock);
        }
    }
    public void Instantiate_Rock()
    {
        current_rocks_in_scene += 1;
        Instantiate(prefab_rock, gameobject_spawn_rocks.transform.position, Quaternion.identity);
    }
}

