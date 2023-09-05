using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_rocks_lever_down : MonoBehaviour
{
    //GameObject pos ;
    //Vector3 maxpos = new Vector3(0f,0f,0f);
    public GameObject prefab_rock;
    public GameObject spawn_pos;
    public Transform spawn_pos1;
    void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        Invoke("Instantiat", 5.0f);
    }

    void Instantiat()
    {
        //Instantiate(prefab_rock, spawn_pos, Quaternion.identity);
    }
}

