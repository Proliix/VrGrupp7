using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OutOfBoundsChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with " + other.gameObject.name, other);

        if (other.GetComponent<Respawnable>() != null)
        {
            other.GetComponent<Respawnable>().Respawn();
            return;
        }

        if (other.GetComponent<XRBaseInteractable>() != null)
            Destroy(other.gameObject);
    }
}
