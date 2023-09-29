using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Crusher : MonoBehaviour
{
    [SerializeField] private float sharpness = 1;
    private float damage;

    Vector3 oldPosition;
    private Rigidbody rb;
    private Collider _collider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oldPosition = transform.position;
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rb.IsSleeping())
        {
            damage = GetForce();
            oldPosition = transform.position;
        }
    }

    float GetForce()
    {
        return (transform.position - oldPosition).magnitude / Time.deltaTime;
    }

    public float GetDamage()
    {
        if (rb.IsSleeping())
        {
            Debug.Log("Crusher is not moving: dealing 0 damage");
            return 0;
        }

        return damage * sharpness;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Crusher Collided with " + other.transform.name);

        if (other.transform.TryGetComponent(out ParentCrushable parentCrushable))
        {
            parentCrushable.CollidedWithCrusher(other, GetDamage(), _collider.bounds.center);
        }
    }

    public void OnGrab()
    {

    }

    public void OnRelease()
    {

    }
}
