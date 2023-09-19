using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAdioOnTriggerEnter : MonoBehaviour
{
    public AudioClip clip_rockAgainstsrock;
    public AudioClip clip_rockAgainstTable;
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
     void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "rocks")
        {
            float volume = Calculate_velocity();
            source.PlayOneShot(clip_rockAgainstsrock, volume);
        }

        if(other.tag == "Table")
        {
            float volume = Calculate_velocity();
            volume -= 0.5f;
            source.PlayOneShot(clip_rockAgainstTable, volume);
        }
    }

}
