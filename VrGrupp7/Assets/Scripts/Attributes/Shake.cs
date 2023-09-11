using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shake : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Vector3 oldPosition;

    public float weakShake = 5;
    public float mediumShake = 15;
    public float strongShake = 25;

    // Start is called before the first frame update
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
            float shakeForce = GetShakeForce();
            oldPosition = transform.position;

            //Debug.Log(shakeForce);

            if (shakeForce > strongShake)
            {
                Debug.Log("Strong Shake");
                return;
            }
            if (shakeForce > mediumShake)
            {
                Debug.Log("Medium Shake");
                return;
            }
            if (shakeForce > weakShake)
            {
                Debug.Log("Weak Shake");
                return;
            }
        }
    }

    //Can be optimized with magnitude squared
    float GetShakeForce()
    {
        return (transform.position - oldPosition).magnitude / Time.deltaTime;
    }
}
