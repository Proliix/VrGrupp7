using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Explosive : MonoBehaviour, IScannable
{

    Rigidbody m_rb;
    [Range(0f, 20f)] public float forceRequiredToExplode = 5f;
    [SerializeField] private GameObject explosion;

    private void OnCollisionEnter(Collision other)
    {
        float impactForce = other.impulse.magnitude;

        if(impactForce > forceRequiredToExplode)
        {
            Explode();
        }
    }

    void Explode()
    {
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(newExplosion, 1);
    }

    public string GetScanInformation()
    {
        return "Explosive!";
    }
}
