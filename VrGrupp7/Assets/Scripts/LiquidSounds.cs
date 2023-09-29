using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LiquidEffect))]
public class LiquidSounds : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] AudioClip[] liquidSoundsSway;
    [SerializeField] AudioSource sourceSway;
    [SerializeField] float timeForNextSound = 0.25f;
    [SerializeField] float velocityChangeThreshold = 2.0f;
    [SerializeField] float sloshingThreshold = 0.5f;
    [SerializeField] float sloshFactor = 0.1f;
    [Header("Pour in Settings")]
    [SerializeField] AudioClip[] liquidSoundsPour;
    [SerializeField] AudioSource sourcePour;
    [SerializeField] float minPitch = 0.7f;
    [SerializeField] float maxPitch = 1.2f;
    [SerializeField] float minVolume = 0.5f;
    [SerializeField] float maxVolume = 1;
    [SerializeField] float testDelay = 0.01f;

    LiquidEffect liquid;

    //pour
    bool canPlayPour = true;

    //Sway
    private Vector3 lastPos;
    float swaySoundTimer;
    private float wobbleAmountX;
    private float wobbleAmountZ;

    private void Start()
    {
        liquid = GetComponent<LiquidEffect>();
    }

    public void UpdateSoundFill(float fill)
    {
        sourcePour.pitch = Mathf.Lerp(minPitch, maxPitch, fill);
        sourcePour.volume = Mathf.Lerp(minVolume, maxVolume, fill);
        if (canPlayPour)
        {
            canPlayPour = false;
            AudioClip clip = liquidSoundsPour[Random.Range(0, liquidSoundsPour.Length)];
            sourcePour.clip = clip;
            sourcePour.Play();
            StartCoroutine(WaitForNextPour(clip.length - testDelay));
        }
    }

    IEnumerator WaitForNextPour(float time)
    {
        yield return new WaitForSeconds(time);
        canPlayPour = true;
    }

    public void UpdateSoundSway(Vector3 lastPosition, float wobbleX, float wobbleZ)
    {
        if (liquid.GetIsEmpty())
            return;

        swaySoundTimer += Time.deltaTime;
        lastPos = lastPosition;
        wobbleAmountX = wobbleX;
        wobbleAmountZ = wobbleZ;

        if (swaySoundTimer >= timeForNextSound)
        {
            swaySoundTimer = 0;
            SoundEffectSway();
        }
    }

    void SoundEffectSway()
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
            if (liquidSoundsSway.Length > 0)
            {
                int randomSoundIndex = Random.Range(0, liquidSoundsSway.Length);
                AudioClip randomSound = liquidSoundsSway[randomSoundIndex];
                // Set the volume and pitch of the audio source.
                sourceSway.volume = volume;
                sourceSway.pitch = pitch + ((1 - liquid.GetLiquid()) * 0.25f);

                // Play the selected sound.
                sourceSway.PlayOneShot(randomSound);
            }
        }

    }

}
