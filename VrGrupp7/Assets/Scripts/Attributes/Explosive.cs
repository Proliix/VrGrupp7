using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

[AddComponentMenu("**Attributes**/Explosive")]
public class Explosive : MonoBehaviour, IScannable, IAttribute
{

    Rigidbody m_rb;
    [Range(0f, 20f)] public float maxForceRequiredToExplode = 20f;

    [Range(0f, 1f)] public float potency = 0;

    [SerializeField] public GameObject explosion;

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

    public string GetScanInformation()
    {
        return "Explosive!";
    }

    public void AddToOther(Transform other)
    {
        Explosive otherExplosive = other.GetComponent<Explosive>();
        otherExplosive = otherExplosive == null ? other.gameObject.AddComponent<Explosive>() : otherExplosive;

        otherExplosive.AddEffect(potency);
    }
    private void AddEffect(float potency)
    {
        this.potency += potency * 0.01f;
    }

    public float GetPotency()
    {
        return potency;
    }

    public string GetName()
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
