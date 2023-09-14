using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_rocks_lever_down : MonoBehaviour
{
    //Script description: When player pulls down the lever and touch invisible trigger box
    //there will spawn an Rock on position of the gamobject called Spawn Rocks.
    public GameObject prefab_rock;
    public GameObject gameobject_spawn_rocks;
    public AudioSource Lever_audioSource;
    public AudioClip lever_sound;
    public AudioSource rock_machin_audioSource;
    public AudioClip rock_machin_sound;
    GameObject[] rocks;
    int number_of_rock_in_scene ;
    int max_rock_in_scene = 30;
    float timer_takes_to_spawn_rock = 4.5f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collision_spawn_rocks" && max_rock_in_scene > number_of_rock_in_scene)
        {
            Lever_audioSource.PlayOneShot(lever_sound);
            Invoke("Play_rock_machin_sound", 0.5f);
            Invoke("Instantiate_Rock", timer_takes_to_spawn_rock);
        }
    }
    public void Update() 
    {
        rocks = GameObject.FindGameObjectsWithTag("rocks");
        number_of_rock_in_scene = rocks.Length;
    }
    public void Instantiate_Rock()
    {
        Instantiate(prefab_rock, gameobject_spawn_rocks.transform.position, Quaternion.identity);
        Debug.Log(number_of_rock_in_scene);
    }
    public void Play_rock_machin_sound()
    {
        rock_machin_audioSource.PlayOneShot(rock_machin_sound);
    }
}

