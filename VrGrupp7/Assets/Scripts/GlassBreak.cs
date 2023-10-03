using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    [SerializeField] GameObject[] Shards;
    [SerializeField] float breakForce = 2;
    [SerializeField] AudioClip[] sounds;

    LiquidContainer container;
    AudioSource source;
    bool isBroken;
    Vector3 breakPos = Vector3.zero;


    AudioSource SetupAudioSource(GameObject obj)
    {
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.playOnAwake = false;
        return audioSource;
    }

    public void BreakBottle()
    {
        container = gameObject.GetComponent<LiquidContainer>();
        container.SetHasCork(true);
        for (int i = 0; i < Shards.Length; i++)
        {
            GameObject shard = Instantiate(Shards[i], transform.position, transform.rotation);
            shard.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(Random.Range(75, 300), transform.position, 2f);
            Destroy(shard, 5);
            if (i == 0)
                source = SetupAudioSource(shard);
        }

        container.ForceStopPour();
        source.pitch = Random.Range(0.9f, 1.2f);
        source.volume = .75f;
        source.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
        Destroy(gameObject, 0.05f);
    }


    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude * Time.fixedDeltaTime;
        if (force > breakForce && !isBroken)
        {
            //Makes it so it gets force from below when it breaks
            //breakPos = collision.GetContact(0).point;
            //if (breakPos == Vector3.zero || breakPos == null)
            //    breakPos = transform.position;
            isBroken = true;
            BreakBottle();
        }
    }


}
