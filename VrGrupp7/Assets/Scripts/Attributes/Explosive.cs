using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Explosive")]
public class Explosive : BaseAttribute
{

    Rigidbody m_rb;
    [Range(0f, 0.5f)] public float maxForceRequiredToExplode = 0.5f;


    public GameObject explosion;

    float minimumExplodeThreshold = 0.05f; 
    bool isInvincible = true;
    float invincibleAfterSpawnTime = 0.2f;

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (isInvincible)
    //        return;

    //    float impactForce = other.impulse.magnitude;

    //    if(impactForce > GetForceRequiredToExplode())
    //    {
    //        Explode();
    //    }
    //}

    void TryExplode(float force)
    {
        //If not held by player, return
        if (!m_rb.isKinematic) { return; }

        Debug.Log("Shakeforce = " + force + " required force: " + GetForceRequiredToExplode());

        if(force > GetForceRequiredToExplode())
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
        return Mathf.Max(maxForceRequiredToExplode * (1 - potency), minimumExplodeThreshold);
    }

    void Explode()
    {
        if(TryGetComponent(out GlassBreak glassBreak))
        {
            glassBreak.BreakBottle();
        }

        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);

        if(gameObject.TryGetComponent(out Respawnable respawnable))
        {
            respawnable.Respawn();
        }
        else
        {
            Destroy(gameObject);
        }

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

        var shake = GetComponent<Shake>();

        if(shake == null)
        {
            shake = gameObject.AddComponent<Shake>();
        }

        m_rb = GetComponent<Rigidbody>();


        Invoke(nameof(AddShake), 0.1f);
    }

    void AddShake(Shake shake)
    {
        shake.onShake.AddListener(TryExplode);
    }

    private void TurnOffInvincible()
    {
        isInvincible = false;
    }
}
