using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI displayText;

    //[SerializeField] private string text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IScannable[] effects = other.GetComponents<IScannable>();

        if(effects == null)
        {
            displayText.text = "This Object doesn't have any effects";
            return;
        }

        string output = "";
        foreach(IScannable effect in effects)
        {
            output += (effect.GetScanInformation() + '\n');
        }

        displayText.text = output;
    }

    private void OnTriggerExit(Collider other)
    {
        displayText.text = "Empty";
    }

}
