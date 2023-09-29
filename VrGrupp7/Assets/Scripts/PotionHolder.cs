using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PotionHolder : MonoBehaviour
{
    XRGrabInteractable interactable;
    LiquidContainer container;
    bool holdingPotion;
    bool selectedInTrigger;
    Rigidbody rbd;


    void HasContainer(SelectExitEventArgs args)
    {
        holdingPotion = true;
        if (container.gameObject.TryGetComponent(out rbd))
        {
            rbd.isKinematic = true;
        }
        container.transform.position = transform.position;
        interactable.selectEntered.AddListener(RemoveContainer);
        interactable.selectExited.RemoveListener(HasContainer);
    }

    void RemoveContainer(SelectEnterEventArgs args)
    {
        interactable.selectExited.AddListener(ForceKinematicOff);
        holdingPotion = false;
        interactable.selectEntered.RemoveListener(RemoveContainer);
    }


    void ForceKinematicOff(SelectExitEventArgs args)
    {
        rbd.isKinematic = false;
        interactable.selectExited.RemoveListener(ForceKinematicOff);
    }

    private void OnTriggerEnter(Collider other)
    {
        XRGrabInteractable tmpInteractable = null;
        if (!holdingPotion)
        {
            if (!selectedInTrigger)
                if (other.gameObject.TryGetComponent(out tmpInteractable))
                {
                    if (tmpInteractable.isSelected && other.gameObject.TryGetComponent(out container))
                    {
                        selectedInTrigger = true;
                        interactable = tmpInteractable;
                        interactable.selectExited.AddListener(HasContainer);
                    }
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == interactable?.gameObject)
        {
            selectedInTrigger = false;
            interactable.selectExited.RemoveListener(HasContainer);
        }

    }
}
