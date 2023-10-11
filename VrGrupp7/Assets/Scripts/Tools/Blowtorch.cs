using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blowtorch : MonoBehaviour
{
    [SerializeField] ParticleSystem fire;
    private List<Torchable> insideTorch = new List<Torchable>();

    private bool isActive = false;

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
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out Torchable torchable))
        {
            if (!insideTorch.Contains(torchable))
                insideTorch.Add(torchable);

            if (isActive)
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
