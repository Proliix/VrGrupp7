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
        scannerDisplay.SetActive(true);

        Transform interactableTransform = args.interactableObject.transform;

        string scanData = GetScanData(interactableTransform);

        SetDisplayText(scanData);
        SetDisplayTransform(interactableTransform);


        //Debug.Log($"{args.interactorObject} hovered over {args.interactableObject}", this);
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        scannerDisplay.SetActive(false);

        //Debug.Log($"{args.interactorObject} stopped hovering over {args.interactableObject}", this);

        //SetDisplayText("Waiting for input...");
    }
}
