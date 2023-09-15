using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
//[RequireComponent(typeof(CanHaveAttributes))]

[AddComponentMenu("**Attributes**/Transparency")]
public class Transparency : MonoBehaviour, IScannable, IAttribute
{
    [Range(0, 1f)] public float transparencyModifier = 1;
    [SerializeField][Range(0, 1f)] private float baseTransparency = 1;

    bool isTransparent = false;
    [SerializeField] private Material m_transparent;

    private Material m_current;
    private Material m_old;

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
        //If we try to change transparency without reference to the m_transparent material we stop
        //This only happens if we use AddComponent and this code runs before we assign the m_transparent material
        if(m_transparent == null) { return; }

        //Copy current material if we don't have a copy of it
        if(m_current == null)
        {
            m_old = GetComponent<Renderer>().material;
            m_current = new Material(m_old);

            baseTransparency = m_current.color.a;
        }

        //Check if material is transparent by looking at alpha value of the color
        isTransparent = m_current.color.a != 1;

        if (!isTransparent)
        {
            m_current = ChangeToTransparentMode(m_current);
        }

        //
        Color newColor = m_current.color;
        newColor.a = GetTransparency();
        m_current.color = newColor;

        GetComponent<Renderer>().material = m_current;
    }

    Material ChangeToTransparentMode(Material material)
    {
        Material newMaterial = new Material(m_transparent);
        newMaterial.color = material.color;
        isTransparent = true;

        return newMaterial;
    }

    void ChangeToOriginal()
    {
        if(!isTransparent) { return; }

        isTransparent = false;
        GetComponent<Renderer>().material = m_old;
        m_current = null;
        m_old = null;
    }
    float GetTransparency()
    {
        return baseTransparency * transparencyModifier;
    }

    public string GetScanInformation()
    {
        return "Transparency: " + ((int)(GetTransparency() * 100)) + "%";
    }

    //Use when addcomponent is used
    public void AddTrasparentMat(Material mat)
    {
        m_transparent = mat;
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

        otherTransparent.AddEffect(transparencyModifier);
    }

    public void AddEffect(float potency)
    {
        transparencyModifier = Mathf.MoveTowards(transparencyModifier, potency, 0.05f * Time.deltaTime);
        ChangeTransparency();
    }


}
