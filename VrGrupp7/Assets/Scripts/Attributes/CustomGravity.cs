using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(Rigidbody))]

[AddComponentMenu("**Attributes**/CustomGravity")]
public class CustomGravity : BaseAttribute
{
    // Gravity Scale editable on the inspector
    // providing a gravity scale per object

    //[SerializeField] private float potency = 0;

    private float throwVelocity;
    private float throwAngular;

    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.

    public static float globalGravity = -9.81f;

    Rigidbody m_rb;

    private void Start()
    {
        //SetGravity(gravityScale);
    }

    void OnEnable()
    {
        m_rb = GetComponent<Rigidbody>();

        if(m_rb == null)
        {
            m_rb = gameObject.AddComponent<Rigidbody>();
        }

        if(TryGetComponent(out Renderer renderer))
        {
            m_rb.mass = renderer.bounds.size.magnitude * 10f;
        }

        m_rb.useGravity = false;

        SetGravity(GetGravityModifier());
    }

    void OnDisable()
    {
        DisableCustomGravity();
    }

    private void OnDestroy()
    {
        DisableCustomGravity();
    }

    void FixedUpdate()
    {
        if (m_rb.useGravity == true)
            m_rb.useGravity = false;

        Vector3 gravity = globalGravity * GetGravityModifier() * Vector3.up;
        m_rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void SetGravity(float gravityModifier)
    {
        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            //Save default settings
            throwVelocity = grabInteractable.throwVelocityScale;
            throwAngular = grabInteractable.throwAngularVelocityScale;

            grabInteractable.throwVelocityScale = 1f * gravityModifier + 0.5f;
            grabInteractable.throwAngularVelocityScale = 1f * gravityModifier;
        }
    }

    float GetGravityModifier()
    {
        return 1 - Mathf.Clamp01(potency);
    }

    void DisableCustomGravity()
    {
        if (TryGetComponent(out m_rb))
            m_rb.useGravity = true;

        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.throwVelocityScale = throwVelocity;
            grabInteractable.throwAngularVelocityScale = throwAngular;
        }
    }

    public override string GetScanInformation()
    {
        return "Custom Gravity: " + (potency * 100) + "%";
    }

    public override string GetName()
    {
        return "Custom Gravity";
    }
}