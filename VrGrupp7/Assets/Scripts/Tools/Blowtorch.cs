using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blowtorch : MonoBehaviour
{

    [SerializeField] ParticleSystem fire;
    List<Torchable> insideTorch = new List<Torchable>();

    bool isActive = false;

    [SerializeField] AudioClip start;
    [SerializeField] AudioClip loop;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartTorch()
    {
        fire.Play();
        isActive = true;

        audioSource.PlayOneShot(start);
        audioSource.clip = loop;
        audioSource.PlayDelayed(start.length - 0.5f);


        for (int i = 0; i < insideTorch.Count; i++)
        {
            insideTorch[i].OnTorchEnter();

            //Debug.Log("Starting torch " + insideTorch[i].name);
        }
    }

    public void StopTorch()
    {
        fire.Stop();
        isActive = false;

        audioSource.Stop();

        for (int i = 0; i < insideTorch.Count; i++)
        {
            if (insideTorch[i] == null)
            {
                continue;
            }
            insideTorch[i].OnTorchExit();

            //Debug.Log("Stopping torch " + insideTorch[i].name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out Torchable torchable))
        {
            if (!insideTorch.Contains(torchable))
                insideTorch.Add(torchable);

            if(isActive)
                torchable.OnTorchEnter();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Torchable torchable))
        {
            insideTorch.Remove(torchable);

            torchable.OnTorchExit();
        }
    }
}
