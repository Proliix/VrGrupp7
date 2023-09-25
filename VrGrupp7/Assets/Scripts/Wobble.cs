using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    [SerializeField] AudioClip[] liquidSounds;

    Renderer rend;
    AudioSource source;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastVelocity;
    Vector3 lastRot;
    Vector3 lastAngularVelocity;
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    public float timeForNextSound = 0.25f;
    float soundTimer;
    public float velocityChangeThreshold = 2.0f;
    public float sloshingThreshold = 0.5f;
    public float sloshFactor = 0.1f;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        source = GetComponent<AudioSource>();
    }

    void SoundEffect()
    {
        // Calculate the change in position since the last frame
        Vector3 positionChange = transform.position - lastPos;

        // Calculate the speed of the movement
        float movementSpeed = positionChange.magnitude / Time.deltaTime;

        // Calculate the magnitude of wobbling to determine slosh intensity
        float wobbleMagnitude = Mathf.Max(Mathf.Abs(wobbleAmountX), Mathf.Abs(wobbleAmountZ));

        // Calculate the combined intensity based on both velocity change and sloshing
        float combinedIntensity = Mathf.Max(movementSpeed / velocityChangeThreshold, wobbleMagnitude);

            // Adjust volume and pitch based on the combined intensity
            float minVolume = 0.1f;
            float maxVolume = 1.0f;
            float minPitch = 0.7f;
            float maxPitch = 1.3f;

            // Map the combinedIntensity to volume and pitch using Lerp
            float volume = Mathf.Lerp(minVolume, maxVolume, combinedIntensity * sloshFactor);
            float pitch = Mathf.Lerp(minPitch, maxPitch, combinedIntensity * sloshFactor);
        
        // Check if the combined intensity exceeds the sloshing threshold
        if (combinedIntensity > sloshingThreshold)
        {

            // Play a random liquid sound from the array.
            if (liquidSounds.Length > 0)
            {
                int randomSoundIndex = Random.Range(0, liquidSounds.Length);
                AudioClip randomSound = liquidSounds[randomSoundIndex];
                // Set the volume and pitch of the audio source.
                source.volume = volume;
                source.pitch = pitch;

                // Play the selected sound.
                source.PlayOneShot(randomSound);
            }
        }

    }

    private void Update()
    {
        time += Time.deltaTime;
        soundTimer += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);


        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        //Check if it should play a sound effect
        if (source != null && soundTimer > timeForNextSound)
        {
            soundTimer = 0;
            SoundEffect();
        }

        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
        lastVelocity = velocity;
        lastAngularVelocity = angularVelocity;
    }



}