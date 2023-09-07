using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class Transparency : MonoBehaviour
{
    [Range(0,1f)] public float transparency = 1;

    bool isTransparent = false;
    [SerializeField] private Material transparent;
    private Material oldMaterial;

    // Start is called before the first frame update
    void OnEnable()
    {
        ChangeTransparency();
    }
    private void OnDisable()
    {
        ChangeToOriginal();
    }

    void ChangeTransparency()
    {
        Material material = new Material(GetComponent<Renderer>().material);

        if (!isTransparent)
        {
            material = ChangeToTransparentMode(material);
        }

        Color newColor = material.color;
        newColor.a = transparency;
        material.color = newColor;

        GetComponent<Renderer>().material = material;
    }

    Material ChangeToTransparentMode(Material material)
    {
        oldMaterial = material;
        Material newMaterial = new Material(transparent);
        newMaterial.color = material.color;
        isTransparent = true;

        return newMaterial;
    }

    void ChangeToOriginal()
    {
        if(!isTransparent) { return; }

        isTransparent = false;
        GetComponent<Renderer>().material = oldMaterial;
    }


}
