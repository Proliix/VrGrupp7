using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GenericSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnObj;
    [SerializeField] Vector3 spawnPos;
    [SerializeField] float maxObj = 10;
    [Header("Audio")]
    [Tooltip("Can be null")]
    [SerializeField] AudioClip spawnSound;
    [SerializeField] float pitchMin = 1, pitchMax = 1;

    AudioSource audioSource;
    List<GameObject> objects = new List<GameObject>();


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Spawn()
    {
        RemoveFromList();

        if (objects.Count < maxObj)
        {
            if (spawnSound != null)
            {
                audioSource.pitch = Random.Range(pitchMin, pitchMax);
                audioSource.PlayOneShot(spawnSound);
            }
            GameObject newObj = Instantiate(spawnObj, spawnPos + transform.position, spawnObj.transform.rotation);
            objects.Add(newObj);
        }
    }
    void RemoveFromList()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i] == null)
                objects.RemoveAt(i);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(spawnPos + transform.position, Vector3.one * 0.05f);
    }
}
