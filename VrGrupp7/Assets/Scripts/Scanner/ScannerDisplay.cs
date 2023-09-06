using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ScannerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;

    private Transform trackTransform;

    public Transform headTransform;

    // Start is called before the first frame update
    void Start()
    {
        if(headTransform == null)
        {
            headTransform = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackTransform == null) { return; }

        transform.position = trackTransform.position;
        transform.rotation = headTransform.rotation;

        //transform.rotation = Quaternion.LookRotation(headTransform.position);
    }

    public void SetText(string text)
    {
        tmpText.text = text;
    }

    public void SetTransform(Transform interactable)
    {
        trackTransform = interactable;
    }
}
