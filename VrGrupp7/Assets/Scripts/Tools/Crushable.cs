using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class Crushable : MonoBehaviour
{
    public AudioClip clip_soundWhenHit;

    [SerializeField] private GameObject[] detatchOnDestroy;

    [HideInInspector] public float startHealth;
    public float currentHealth;
    [SerializeField] private float heatModifier = 3f;

    bool isInvincible = false;
    float invincibleAfterHitTime = 0.1f;

    public GameObject ParticleOnHit;

    //This value controls on how much the color varies from the base color, 0 means all particles have same color as base;
    public float particleColorGradient = 0.5f;


    void Start()
    {
        startHealth = currentHealth;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Crushable Collided with " + other.transform.name);

        if (other.transform.TryGetComponent(out Crusher crusher))
        {
            OnCollision(crusher.GetDamage() ,other.GetContact(0).point, other.collider.bounds.center);
        }
    }

    public void OnCollision(float damage, Vector3 hitLocation, Vector3 crusherLocation)
    {
        if (!isInvincible)
        {
            //float damage = crusher.GetDamage();

            //AudioSource.PlayClipAtPoint(clip_soundWhenHit, transform.position, damage);

            LoseHealth(damage);

            StartCoroutine(Invincible(invincibleAfterHitTime));

            SpawnParticleEffect(damage, hitLocation, crusherLocation);

        }
    }


    public void LoseHealth(float damage)
    {
        //Debug.Log("Damage: " + damage + " to " + gameObject.name);

        if(TryGetComponent(out Torchable torchable))
        {
            damage *= 1 + torchable.GetTemperature() * heatModifier;
        }


        currentHealth -= damage;

        if(currentHealth < 0)
        {
            Crush();
        }
    }

    void Crush()
    {

        foreach(GameObject obj in detatchOnDestroy)
        {
            if(obj == null) { continue; }

            obj.transform.parent = null;

            if(obj.TryGetComponent(out AddGrab addGrab))
            {
                addGrab.Add();
            }
        }

        Destroy(gameObject);
    }

    IEnumerator Invincible(float time)
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }

    void SpawnParticleEffect(float damage, Vector3 hitLocation, Vector3 crusherLocation)
    {
        //Get hit location
        //Vector3 hitLocation = other.GetContact(0).point;
        //Spawn particle
        GameObject particle = Instantiate(ParticleOnHit, hitLocation, Quaternion.identity);

        particle.transform.localScale = Vector3.one * Mathf.Clamp(damage, 0, 1);

        //Get Color of our gameobject
        var color = GetComponent<Renderer>().material.color;

        //Create a light/dark gradient from our gameobject color
        var gradient = new ParticleSystem.MinMaxGradient(color * (1 - particleColorGradient), color * (1 + particleColorGradient));
        var main = particle.GetComponent<ParticleSystem>().main;
        //Set particle color to gradient;
        main.startColor = gradient;

        //particle.GetComponent<ParticleSystemRenderer>().material = GetComponent<Renderer>().material;

        //Rotate the particle effect to the impact direction
        particle.transform.up = crusherLocation - hitLocation;
        //Destroy after 1 second;
        Destroy(particle, 1);
    }

}
