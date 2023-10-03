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
    bool canPlaySound = true;

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
        container.SetHasCork(false);
        if (canPlaySound)
        {
            audioSorce.pitch = Random.Range(0.9f, 1.1f);
            audioSorce.Play();
            canPlaySound = false;
            Invoke(nameof(ResetPlaySound),0.25f);
        }
    }

    void ResetPlaySound()
    {
        canPlaySound = true;
    }

    void AttachCork(SelectEnterEventArgs args)
    {
        container.SetHasCork(true);
    }


}