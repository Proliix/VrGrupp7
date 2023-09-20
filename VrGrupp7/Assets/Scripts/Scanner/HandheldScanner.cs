using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class HandheldScanner : MonoBehaviour
{
    public GameObject scannerDisplay;

    private Transform interactableTransform;

    void Start()
    {
        ////We spawn the display that pops up next to the scanned object
        //scannerDisplay = Instantiate(scannerDisplay);
        ////We disable it so it's hidden
        //scannerDisplay.SetActive(false);

        SetDisplayText("Point at a affected object to check its effects!");
    }

    public string GetScanData(Transform transform)
    {
        //Get all scannable interfaces that the transform has
        IScannable[] effects = transform.GetComponents<IScannable>();

        string output = "Effects:\n";

        if (effects == null)
        {
            output = "This Object doesn't have any effects";
            return output;
        }

        //Get Scan info from each scannable component attached to the transform, seperate them with a new line
        foreach (IScannable effect in effects)
        {
            output += (effect.GetScanInformation() + '\n');
        }

        return output;
    }

    void SetDisplayText(string text)
    {
        scannerDisplay.GetComponent<ScannerDisplay>().SetText(text);
    }

    void SetDisplayTransform(Transform interactable)
    {
        scannerDisplay.GetComponent<ScannerDisplay>().SetTransform(interactable);
    }


    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        interactableTransform = args.interactableObject.transform;

        if (!CheckIfScannable(interactableTransform))
        {
            return;
        }

        //Add listener to the scanned objects attribute, if the value changes we update the scanner
        if (interactableTransform.TryGetComponent(out CanHaveAttributes attributes))
        {
            attributes.onValueChanged.AddListener(UpdateScanner);
        }

        //We update the scanners value when we hover over a new object
        UpdateScanner();

        //We enable the scanner display gameobject
        scannerDisplay.SetActive(true);

        //Debug.Log($"{args.interactorObject} hovered over {args.interactableObject}", this);
    }

    void UpdateScanner()
    {
        string scanData = GetScanData(interactableTransform);

        SetDisplayText(scanData);
        SetDisplayTransform(interactableTransform);
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        //If no scannable components was found we ignore it
        if (!CheckIfScannable(args.interactableObject.transform))
        {
            return;
        }

        //We remove the listener to the event
        if (interactableTransform.TryGetComponent(out CanHaveAttributes attributes))
        {
            attributes.onValueChanged.RemoveListener(UpdateScanner);
        }

        //if (scannerDisplay != null)
        //    scannerDisplay.SetActive(false);

        //Debug.Log($"{args.interactorObject} stopped hovering over {args.interactableObject}", this);
        //SetDisplayText("Waiting for input...");
    }

    private bool CheckIfScannable(Transform transform)
    {
        return transform.TryGetComponent<IScannable>(out _);
    }
}
