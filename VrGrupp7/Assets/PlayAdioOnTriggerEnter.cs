using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayAdioOnTriggerEnter : MonoBehaviour
{
    public AudioClip clip_rockAgainstsrock;
    public AudioClip clip_rockAgainstTable;

    public AudioClip clip_rockAgainstfloor;
    float timer;

    
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    float Calculate_velocity()
    {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 v3Velocity = rb.velocity;
            float speed = v3Velocity.magnitude;

            //kolla om jag kan få public float minVelocity = 0; public float maxVelocity = 10; istället för 0 och 5
            float volume = Mathf.InverseLerp(0, 5, speed);
            return volume;
    }

    private void Update() 
    {
        timer += Time.deltaTime;
    }
     void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "rocks" && timer > 1)
        {
            float volume = Calculate_velocity();
            source.PlayOneShot(clip_rockAgainstsrock, volume);
        }

        if(other.tag == "Table" && timer > 1)
        {
            float volume = Calculate_velocity();
            source.PlayOneShot(clip_rockAgainstTable, volume);
        }
        if(other.tag == "floor" && timer > 1)
        {
            float volume = Calculate_velocity();
            source.PlayOneShot(clip_rockAgainstfloor, volume);
        }
    }

}
