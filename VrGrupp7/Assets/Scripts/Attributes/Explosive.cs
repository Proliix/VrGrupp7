using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Explosive")]
public class Explosive : BaseAttribute
{
    [Range(0f, 0.5f)][SerializeField] 
    private float maxForceRequiredToExplode = 0.5f;
    public GameObject explosion;

    private Rigidbody m_rb;
    private float minimumExplodeThreshold = 0.05f;
    private float invincibleAfterSpawnTime = 0.2f;
    private bool isInvincible = true;

    private Shake shake;

    private void OnEnable()
    {
        isInvincible = true;
        Invoke(nameof(TurnOffInvincible), invincibleAfterSpawnTime);

        shake = GetComponent<Shake>();

        if (shake == null)
        {
            shake = gameObject.AddComponent<Shake>();
            shake.onShake = new UnityEngine.Events.UnityEvent<float>();
        }

        m_rb = GetComponent<Rigidbody>();
        shake.onShake.AddListener(TryExplode);
    }

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

    private void TurnOffInvincible()
    {
        isInvincible = false;
    }
}
