using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Respawnable : MonoBehaviour
{
    Vector3 position;
    Quaternion rotation;

    //FIX FOR PLAYER
    CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        rotation = transform.rotation;

        TryGetComponent(out charController);
    }

    public void Respawn()
    {
        XRGrabInteractable grabInteractable;

        if (TryGetComponent(out grabInteractable))
        {
            grabInteractable.enabled = false;
        }

        Debug.Log("Respawned " + gameObject.name, this);

        if (charController != null)
            charController.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        if (charController != null)
            charController.enabled = true;

        if(TryGetComponent(out CanHaveAttributes canHaveAttributes))
        {
            canHaveAttributes.RemoveAllAttributes();
        }

        if (TryGetComponent(out grabInteractable))
        {
            grabInteractable.enabled = true;
        }
    }

}
