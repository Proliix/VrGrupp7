using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AddGrab : MonoBehaviour
{
    [SerializeField] InteractionLayerMask layerMask;
    [SerializeField] private bool setColliderToTriggerOnSelect = false;

    public void Add()
    {
        StartCoroutine(IEAddGrab());
    }

    //Waits for 1 frame due to the XR Grab needing to unsubscribe the collider from the parents XR Grab
    IEnumerator IEAddGrab()
    {
        yield return 0;

        if(GetComponent<XRGrabInteractable>() == null)
        {
            var grabInteractable = gameObject.AddComponent<XRGrabInteractable>();

            grabInteractable.interactionLayers = layerMask;

            if (setColliderToTriggerOnSelect)
            {
                grabInteractable.selectEntered.AddListener(SetColliderTriggersOn);
                grabInteractable.selectExited.AddListener(SetColliderTriggersOff);

            }
        }
    }

    private void SetColliderTriggersOff(SelectExitEventArgs arg0)
    {
        SetColliderTrigger(false);
    }

    private void SetColliderTriggersOn(SelectEnterEventArgs arg0)
    {
        SetColliderTrigger(true);
    }

    void SetColliderTrigger(bool enabled)
    {
        foreach(Collider col in GetComponents<Collider>())
        {
            col.isTrigger = enabled;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.isTrigger = enabled;
        }
    }
}
