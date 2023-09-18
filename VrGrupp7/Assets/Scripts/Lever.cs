using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRSimpleInteractable))]
[HelpURL("https://www.youtube.com/watch?v=ZaWu0YPmDJo")]
public class Lever : MonoBehaviour
{
    [SerializeField] float angleForStart;
    [SerializeField] float clampMin, clampMax;
    [SerializeField] Vector3 dotDir = Vector3.right;
    public UnityEvent onEnable;
    public UnityEvent onDisable;


    Vector3 startPos;
    Quaternion startRot;
    Rigidbody rbd;
    bool isActive;
    bool isGrabbed;

    GameObject hand;

    XRSimpleInteractable interactable;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        rbd = GetComponent<Rigidbody>();
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(GrabLever);
        interactable.selectExited.AddListener(ResetPos);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbed)
        {
            Vector3 dir = hand.transform.position - transform.position;
            float dot = Vector3.Dot(dir, dotDir);

            if (dot > 0)
            {
                transform.LookAt(new Vector3(hand.transform.position.x, hand.transform.position.y, transform.position.z));
                transform.eulerAngles = new Vector3(ClampAngle(transform.rotation.eulerAngles.x, clampMin, clampMax), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

        }

        if (Vector3.Angle(transform.up, Vector3.left) > angleForStart)
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

    void GrabLever(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        hand = args.interactorObject.transform.gameObject;
    }

    void ResetPos(SelectExitEventArgs args)
    {
        isGrabbed = false;
        transform.position = startPos;
        transform.rotation = startRot;
        rbd.velocity = Vector3.zero;
    }
}
