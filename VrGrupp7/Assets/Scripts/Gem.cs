using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out IAttribute attribute) && 
            TryGetComponent(out Torchable torchable))
        {
            var colors = PotionColors.GetColor(attribute);
            torchable.maxTorchedColor = colors.sideColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
