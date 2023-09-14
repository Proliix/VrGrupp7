using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class Crushable : MonoBehaviour
{
    public float health;

    bool isInvincible = false;
    float invincibleAfterHitTime = 0.1f;

    public GameObject ParticleOnHit;

    //This value controls on how much the color varies from the base color, 0 means all particles have same color as base;
    public float particleColorGradient = 0.5f;

    private void OnCollisionEnter(Collision other)
    {

        if(other.transform.TryGetComponent(out Crusher crusher))
        {
            if (!isInvincible)
            {
                float damage = crusher.GetDamage();

                LoseHealth(damage);

                StartCoroutine(Invincible(invincibleAfterHitTime));

                SpawnParticleEffect(other);

            }
        }
    }

    void LoseHealth(float damage)
    {
        Debug.Log("Damage: " + damage + " to " + gameObject.name);

        health -= damage;

        if(health < 0)
        {
            Crush();
        }
    }

    void Crush()
    {
        Gem[] gems = transform.GetComponentsInChildren<Gem>();

        foreach(Gem gem in gems)
        {
            gem.transform.parent = transform.parent;
            gem.AddGrab();
        }

        Destroy(gameObject);
    }

    IEnumerator Invincible(float time)
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }

    void SpawnParticleEffect(Collision other)
    {
        //Get hit location
        Vector3 hitLocation = other.GetContact(0).point;
        //Spawn particle
        GameObject particle = Instantiate(ParticleOnHit, hitLocation, Quaternion.identity);

        //Get Color of our gameobject
        var color = GetComponent<Renderer>().material.color;

        //Create a light/dark gradient from our gameobject color
        var gradient = new ParticleSystem.MinMaxGradient(color * (1 - particleColorGradient), color * (1 + particleColorGradient));
        var main = particle.GetComponent<ParticleSystem>().main;
        //Set particle color to gradient;
        main.startColor = gradient;

        //particle.GetComponent<ParticleSystemRenderer>().material = GetComponent<Renderer>().material;

        //Rotate the particle effect to the impact direction
        particle.transform.up = other.transform.position - hitLocation;
        //Destroy after 1 second;
        Destroy(particle, 1);
    }

}
