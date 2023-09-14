using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
[HelpURL("https://www.youtube.com/watch?v=ZaWu0YPmDJo")]
public class Lever : MonoBehaviour
{
    [SerializeField] float angleForStart;
    public UnityEvent onEnable;
    public UnityEvent onDisable;


    Vector3 startPos;
    Quaternion startRot;
    Rigidbody rbd;
    bool isActive;

    XRGrabInteractable interactable;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        rbd = GetComponent<Rigidbody>();
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectExited.AddListener(ResetPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(transform.up, Vector3.up) > angleForStart)
        {
            if (!isActive)
            {
                isActive = true;
                onEnable.Invoke();
            }

        }
        else if (isActive)
        {
            isActive = false;
            onDisable.Invoke();
        }
    }
    void ResetPos(SelectExitEventArgs args)
    {
        transform.position = startPos;
        transform.rotation = startRot;
        rbd.velocity = Vector3.zero;
    }
}
