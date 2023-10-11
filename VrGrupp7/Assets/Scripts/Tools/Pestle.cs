using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pestle : MonoBehaviour
{
    [SerializeField]float grindDamage = 1f;
    private bool dealtDamage = false;

    private AudioSource audioSource;
    [SerializeField] private AudioClip grindSound;
    [SerializeField] private float volumeModifier = 1;
    [SerializeField] private float soundChangeSpeed = 1;

    private bool couroutineIsRunning = false;
    private IEnumerator fadeOutSound;

    private Rigidbody rb;
    private Collider _collider;

    private Vector3 oldPosition;
    private Vector3 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = grindSound;
        audioSource.volume = 0;
    }

    void Update()
    {
        if (!rb.IsSleeping())
        {
            velocity = (transform.position - oldPosition);
            oldPosition = transform.position;
        }
    }
    IEnumerator FadeOutSound()
    {
        couroutineIsRunning = true;

        while (couroutineIsRunning)
        {
            yield return null;
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, -0.1f, Time.deltaTime * 5);
            if (audioSource.volume <= 0)
            {
                audioSource.volume = 0;
                fadeOutSound = null;
                couroutineIsRunning = false;
                audioSource.Stop();
            }
        }
    }


    public void SetTrigger(bool isTrigger)
    {
        _collider.isTrigger = isTrigger;
    }

    private float GetEfficiency(Vector3 mortarCenter)
    {
        if (this.velocity == Vector3.zero)
            return 0;

        Vector2 pestleLocation = new Vector2(transform.position.x, transform.position.z);
        Vector2 mortarLocation = new Vector2(mortarCenter.x, mortarCenter.z);

        Vector2 velocity = new Vector2(this.velocity.x, this.velocity.z);
        Vector2 direction = pestleLocation - mortarLocation;
        Vector2 normal = Vector2.Perpendicular(direction);

        float angle = Vector2.Angle(normal, velocity);

        if (angle > 90)
            angle = 180 - angle;

        float efficiency01 = 1 - angle / 90f;

        return efficiency01;
    }

    public float GetDamage(Vector3 mortarCenter)
    {
        if (couroutineIsRunning)
        {
            StopCoroutine(fadeOutSound);
            couroutineIsRunning = false;
        }
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        dealtDamage = true;
        float efficiency01 = GetEfficiency(mortarCenter);
        float damage = grindDamage * efficiency01 * velocity.magnitude;

        float targetVolume = Mathf.Clamp01(damage * volumeModifier);
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * soundChangeSpeed);

        return damage;
    }

    private void LateUpdate()
    {
        if (!dealtDamage)
        {
            if (!couroutineIsRunning)
            {
                fadeOutSound = FadeOutSound();
                StartCoroutine(fadeOutSound);
            }
        }

        dealtDamage = false;
    }
}
