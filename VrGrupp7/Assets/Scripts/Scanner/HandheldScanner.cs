using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class HandheldScanner : MonoBehaviour
{
    public GameObject scannerDisplay;

    void Start()
    {
        scannerDisplay = Instantiate(scannerDisplay);
        scannerDisplay.SetActive(false);
    }

    public string GetScanData(Transform transform)
    {
        IScannable[] effects = transform.GetComponents<IScannable>();

        string output = "Effects:\n";

        if (effects == null)
        {
            output = "This Object doesn't have any effects";
            return output;
        }

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
        Transform interactableTransform = args.interactableObject.transform;

        if(!CheckIfScannable(interactableTransform))
        {
            return;
        }

        string scanData = GetScanData(interactableTransform);

        SetDisplayText(scanData);
        SetDisplayTransform(interactableTransform);

        scannerDisplay.SetActive(true);

        //Debug.Log($"{args.interactorObject} hovered over {args.interactableObject}", this);
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        if (!CheckIfScannable(args.interactableObject.transform))
        {
            return;
        }

        if(scannerDisplay != null)
            scannerDisplay.SetActive(false);

        //Debug.Log($"{args.interactorObject} stopped hovering over {args.interactableObject}", this);

        //SetDisplayText("Waiting for input...");
    }

    private bool CheckIfScannable(Transform transform)
    {
        return transform.TryGetComponent<IScannable>(out _);
    }
}
