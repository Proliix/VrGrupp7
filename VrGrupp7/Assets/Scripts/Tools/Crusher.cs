using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Crusher : MonoBehaviour
{
    [Header("Crusher settings")]
    [SerializeField] private Transform hammerHead;
    [SerializeField] private float sharpness = 1;
    private float damage;

    [Header("Sound Controls")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField][Range(0, 1)] private float volumeModifier = 1;
    [SerializeField] [Range(0, 1)] private float maxVolume = 1;
    private float soundCooldownTime = 0.1f;
    private bool soundCooldown = false;

    private Vector3 oldPosition;
    private Rigidbody rb;

    void Start()
    {
        if (hammerHead == null)
            hammerHead = transform;

        rb = GetComponent<Rigidbody>();
        oldPosition = hammerHead.position;
    }

    void Update()
    {
        if (!rb.IsSleeping())
        {
            damage = GetForce();
            oldPosition = hammerHead.position;
        }
    }

    float GetForce()
    {
        return (hammerHead.position - oldPosition).magnitude / Time.deltaTime;
    }

    public float GetDamage()
    {
        if (rb.IsSleeping())
        {
            Debug.Log("Crusher is not moving: dealing 0 damage");
            return 0;
        }

        return damage * sharpness;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out ParentCrushable parentCrushable))
        {
            parentCrushable.CollidedWithCrusher(other, GetDamage(), hammerHead.position);
            //Debug.Log("Crusher dealt " + GetDamage() + " Damage to " + other.transform.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Crushable crushable))
        {
            float damage = GetDamage();
            PlaySound(crushable.clip_soundWhenHit, damage);
            crushable.OnCollision(damage, other.ClosestPoint(hammerHead.position), hammerHead.position);
        }
    }

    private void PlaySound(AudioClip clip, float damage)
    {
        if(audioSource == null) { return; }

        if (soundCooldown) { return; }

        StartCoroutine(PlaySoundCooldown(soundCooldownTime));

        if (clip == null)
            clip = audioSource.clip;

        audioSource.volume = Mathf.Clamp(damage * volumeModifier, 0, maxVolume);
        audioSource.PlayOneShot(clip, damage);
    }

    private IEnumerator PlaySoundCooldown(float time)
    {
        soundCooldown = true;

        yield return new WaitForSeconds(time);

        soundCooldown = false;
    }
}
