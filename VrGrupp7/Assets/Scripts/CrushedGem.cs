using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushedGem : MonoBehaviour
{
    [SerializeField] private Transform mesh;
    [SerializeField] private float transferRate = 0.1f;
    [SerializeField] private float cooldownTime = 0.5f;
    private bool cooldown = false;

    private float lerp = 1;
    private Vector3 startScale;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        startScale = mesh.localScale;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (cooldown) { return; }

        if (other.TryGetComponent(out LiquidContainer _))
        {
            StartCoroutine(StartCooldown(cooldownTime));
            LoseSize(transferRate);
            TransferAttributes(other.transform, transferRate);
        }
    }

    private void LoseSize(float transferRate)
    {
        lerp -= transferRate;
        mesh.localScale = Vector3.Lerp(Vector3.zero, startScale, lerp);

        audioSource.Play();

        if(lerp <= 0)
        {
            Destroy(gameObject);
        }

    }

    private void TransferAttributes(Transform other, float transferRate)
    {
        foreach(BaseAttribute attribute in GetComponents<BaseAttribute>())
        {
            BaseAttribute bottleAttribute = (BaseAttribute)other.GetComponent(attribute.GetType());

            if (bottleAttribute == null)
            {
                bottleAttribute = attribute.AddToOther(other.transform);
            }

            bottleAttribute.AddMass(attribute.mass * transferRate);
        }
    }

    private IEnumerator StartCooldown(float delay)
    {
        cooldown = true;
        yield return new WaitForSeconds(delay);
        cooldown = false;
    }
}
