using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    [SerializeField] GameObject[] Shards;
    [SerializeField] float breakForce = 2;

    Rigidbody rbd;

    private void Start()
    {
        rbd = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude * Time.fixedDeltaTime;

        Debug.Log("Force: " + force);

        if (force > breakForce)
        {
            Debug.Log("BROKE");
        }
    }


}
