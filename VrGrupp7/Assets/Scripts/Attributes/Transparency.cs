using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CanHaveAttributes))]

[AddComponentMenu("**Attributes**/Transparency")]
public class Transparency : BaseAttribute
{
    public Material m_transparent;

    private bool isTransparent = false;
    private float baseTransparency = 1;

    private Renderer renderer;
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

    public override void UpdateStats()
    {
        ChangeTransparency();
    }

    public override void OnComponentAdd(BaseAttribute originalAttribute)
    {
        Transparency other = (Transparency)originalAttribute;
        m_transparent = other.m_transparent;
    }

    void ChangeTransparency()
    {

        //Copy current material if we don't have a copy of it
        if (m_current == null)
        {
            if(renderer == null && TryGetComponent(out Torchable torchable))
            {
                renderer = torchable.torchedObject.GetComponent<Renderer>();
            }

            if(renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }

            if(renderer == null)
            {
                renderer = GetComponentInChildren<Renderer>();
            }

            m_old = renderer.material;
            m_current = new Material(m_old);

            baseTransparency = m_current.color.a;
        }

        //Check if material is transparent by looking at alpha value of the color
        isTransparent = m_current.color.a != 1;

        if (!isTransparent)
        {
            if (m_transparent != null)
            {

                m_current = ChangeToTransparentMode(m_current);
            }
            else
            {
                Debug.LogWarning("No transparent material was found");
                return;
            }
        }

        Color newColor = m_current.color;
        newColor.a = GetTransparency();
        m_current.color = newColor;

        renderer.material = m_current;

        Debug.Log("Setting transparency to " + m_current.color.a);
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
        if (!isTransparent) { return; }

        isTransparent = false;
        renderer.material = m_old;
        m_current = null;
        m_old = null;
    }
    float GetTransparency()
    {
        return baseTransparency * (1 - potency);
    }

    public override string GetScanInformation()
    {
        return "Transparency: " + ((int)(GetTransparency() * 100)) + "%";
    }

    //Use when addcomponent is used
    public void AddTrasparentMat(Material mat)
    {
        m_transparent = mat;
    }

    public override string GetName()
    {
        return "Transparency";
    }
}
