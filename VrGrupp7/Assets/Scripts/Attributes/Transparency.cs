using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class Transparency : MonoBehaviour, IScannable, IAttribute
{
    [Range(0,1f)] public float transparency = 1;

    bool isTransparent = false;
    [SerializeField] private Material m_transparent;
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
        if(m_transparent == null) { return; }

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
        Material newMaterial = new Material(m_transparent);
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

    public string GetScanInformation()
    {
        return "Transparency: " + ((int)(transparency * 100)) + "%";
    }
    public void AddEffect(float potency)
    {
        transparency = Mathf.MoveTowards(transparency, potency, 0.05f * Time.deltaTime);
        ChangeTransparency();
    }


    public void AddToOther(Transform other)
    {
        Transparency otherTransparent = other.GetComponent<Transparency>();

        if(otherTransparent == null)
        {
            otherTransparent = other.gameObject.AddComponent<Transparency>();
            otherTransparent.m_transparent = m_transparent;
        }

        //otherTransparent = otherTransparent == null ? other.gameObject.AddComponent<Transparency>() : otherTransparent;

        otherTransparent.AddEffect(transparency);
    }
}
