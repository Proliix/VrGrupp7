using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Explosive")]
public class Explosive : BaseAttribute
{

    Rigidbody m_rb;
    [Range(0f, 20f)] public float maxForceRequiredToExplode = 20f;

    public GameObject explosion;

    bool isInvincible = true;
    float invincibleAfterSpawnTime = 0.2f;

    private void OnCollisionEnter(Collision other)
    {
        if (isInvincible)
            return;

        float impactForce = other.impulse.magnitude;

        if(impactForce > GetForceRequiredToExplode())
        {
            Explode();
        }
    }

    public override void OnComponentAdd(BaseAttribute originalAttribute)
    {
        Explosive other = (Explosive)originalAttribute;
        explosion = other.explosion;
    }

    float GetForceRequiredToExplode()
    {
        return maxForceRequiredToExplode * (1 - potency);
    }

    void Explode()
    {
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
        Destroy(newExplosion, 1);
    }

    public override string GetScanInformation()
    {
        string information = "Explosive: ";

        if(potency > 0.8f)
        {
            information += "Very Unstable!";
        }
        else if (potency > 0.5f)
        {
            information += "Unstable";
        }
        else if(potency > 0.2f)
        {
            information += "Little Unstable";
        }
        else
        {
            information += "Stable";
        }

        return information;
    }

    public override string GetName()
    {
        return "Explosive";
    }

    private void OnEnable()
    {
        isInvincible = true;
        Invoke(nameof(TurnOffInvincible), invincibleAfterSpawnTime);
    }

    private void TurnOffInvincible()
    {
        isInvincible = false;
    }
}
