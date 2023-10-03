using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlaskStopper : MonoBehaviour
{
    [SerializeField] GameObject flaskObj;


    AudioSource audioSorce;
    XRSocketInteractor socket;
    LiquidContainer container;

    private void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(AttachCork);
        socket.selectExited.AddListener(RemoveCork);
        container = flaskObj.GetComponent<LiquidContainer>();
    }

    void RemoveCork(SelectExitEventArgs args)
    {
        audioSorce.pitch = Random.Range(0.9f, 1.1f);
        audioSorce.Play();
        container.SetHasCork(false);
    }

    void AttachCork(SelectEnterEventArgs args)
    {
        container.SetHasCork(true);
    }


}