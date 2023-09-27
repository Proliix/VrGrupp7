using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LiquidSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] liquidSounds;
    AudioSource source;
    [SerializeField] float timeForNextSound = 0.25f;
    [SerializeField] float velocityChangeThreshold = 2.0f;
    [SerializeField] float sloshingThreshold = 0.5f;
    [SerializeField] float sloshFactor = 0.1f;

    private Vector3 lastPos;
    float soundTimer;
    private float wobbleAmountX;
    private float wobbleAmountZ;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void UpdateSound(Vector3 lastPosition, float wobbleX, float wobbleZ)
    {
        soundTimer += Time.deltaTime;
        lastPos = lastPosition;
        wobbleAmountX = wobbleX;
        wobbleAmountZ = wobbleZ;

        if (soundTimer >= timeForNextSound)
        {
            soundTimer = 0;
            SoundEffect();
        }
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
        Debug.Log("combined intensity: " + combinedIntensity);

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

}
