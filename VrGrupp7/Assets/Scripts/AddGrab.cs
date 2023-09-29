using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AddGrab : MonoBehaviour
{
    [SerializeField] InteractionLayerMask layerMask;

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
        }
    }
}
