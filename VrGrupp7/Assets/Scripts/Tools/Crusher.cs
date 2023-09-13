using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Crusher : MonoBehaviour
{
    Vector3 oldPosition;
    private Rigidbody rb;

    public float damage;
    public float sharpness = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oldPosition = transform.position;
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
        return damage * sharpness;
    }
}
