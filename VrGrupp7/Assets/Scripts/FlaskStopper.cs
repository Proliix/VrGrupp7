using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlaskStopper : MonoBehaviour
{
    public float distanceToBreak = 0.1f;
    [SerializeField] GameObject stopperObj;
    [SerializeField] GameObject flaskObj;

    Rigidbody stopperRbd;
    XRGrabInteractable stopperInteractable;
    XRGrabInteractable flaskInteractable;
    Joint joint;
    AudioSource audioSorce;
    LiquidContainer container;
    bool isAttatched;

    private void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        flaskInteractable = flaskObj.GetComponent<XRGrabInteractable>();
        //container = flaskObj.GetComponent<LiquidContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttatched)
        {
            if (flaskInteractable.isSelected && stopperInteractable.isSelected)
            {
                float distance = Vector3.Distance(flaskInteractable.gameObject.transform.position, stopperObj.transform.position);

                if (distance > distanceToBreak)
                {
                    RemoveCork();
                }
            }
        }
    }

    void RemoveCork()
    {
        Destroy(joint);
        audioSorce.pitch = Random.Range(0.9f, 1.1f);
        audioSorce.Play();
        isAttatched = false;
    }

    void AttachCork(GameObject cork)
    {
        stopperObj = cork;
        stopperObj.transform.position = gameObject.transform.position;
        stopperObj.transform.rotation = gameObject.transform.rotation;
        stopperRbd = stopperObj.GetComponent<Rigidbody>();
        stopperInteractable = stopperObj.GetComponent<XRGrabInteractable>();
        joint = flaskObj.AddComponent<FixedJoint>();
        joint.connectedBody = stopperRbd;

        //ADD FUNCTIONALITY TO STOP FLASK FROM BEING ABLE TO LOOSE LIQUID WHEN CORK IS ATTACHED

        isAttatched = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cork") && !isAttatched)
        {
            AttachCork(other.gameObject);
        }
    }

}