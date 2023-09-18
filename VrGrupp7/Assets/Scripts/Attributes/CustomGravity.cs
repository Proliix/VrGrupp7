using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]

[AddComponentMenu("**Attributes**/CustomGravity")]
public class CustomGravity : MonoBehaviour, IScannable, IAttribute
{
    // Gravity Scale editable on the inspector
    // providing a gravity scale per object

    public float gravityScale = 1.0f;

    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.

    public static float globalGravity = -9.81f;

    Rigidbody m_rb;

    private void Start()
    {
        SetGravity(gravityScale);
    }

    void OnEnable()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.useGravity = false;
    }

    void OnDisable()
    {
        m_rb.useGravity = true;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        m_rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void SetGravity(float percentage)
    {
        if (TryGetComponent<XRGrabInteractable>(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.throwVelocityScale = 1f * percentage + 0.5f;
            grabInteractable.throwAngularVelocityScale = 1f * percentage;
        }

        gravityScale = percentage;
    }

    public string GetScanInformation()
    {
        return "Custom Gravity: " + (gravityScale * 100) + "%";
    }

    public void AddEffect(float potency)
    {
        gravityScale = Mathf.MoveTowards(gravityScale, potency, 0.05f * Time.deltaTime);
    }

    public void AddToOther(Transform other)
    {
        CustomGravity otherGravity = other.GetComponent<CustomGravity>();
        otherGravity = otherGravity == null ? other.gameObject.AddComponent<CustomGravity>() : otherGravity;

        otherGravity.AddEffect(gravityScale);
    }

}