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
        m_rb.useGravity = false;

        SetGravity(gravityScale);
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
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        m_rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void SetGravity(float percentage)
    {
        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            //Save default settings
            throwVelocity = grabInteractable.throwVelocityScale;
            throwAngular = grabInteractable.throwAngularVelocityScale;

            grabInteractable.throwVelocityScale = 1f * percentage + 0.5f;
            grabInteractable.throwAngularVelocityScale = 1f * percentage;
        }

        gravityScale = percentage;
    }

    void DisableCustomGravity()
    {
        if (m_rb != null)
            m_rb.useGravity = true;

        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.throwVelocityScale = throwVelocity;
            grabInteractable.throwAngularVelocityScale = throwAngular;
        }
    }

    public string GetScanInformation()
    {
        return "Custom Gravity: " + (gravityScale * 100) + "%";
    }

    public void AddEffect(float potency)
    {
        gravityScale = Mathf.MoveTowards(gravityScale, potency, 0.075f * Time.deltaTime);
    }

    public void AddToOther(Transform other)
    {
        CustomGravity otherGravity = other.GetComponent<CustomGravity>();

        //Debug.Log("GetComponent: " + otherGravity);

        otherGravity = otherGravity == null ? other.gameObject.AddComponent<CustomGravity>() : otherGravity;

        //Debug.Log("AddComponent: " + otherGravity);

        otherGravity.AddEffect(gravityScale);
    }

    public float GetPotency()
    {
        return 1 - gravityScale;
    }

    public string GetName()
    {
        return "Custom Gravity";
    }
}