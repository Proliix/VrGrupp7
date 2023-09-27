using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Shake : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Vector3 oldPosition;

    public float weakShakeForce = 5;
    public float mediumShakeForce = 15;
    public float strongShakeForce = 25;

    public UnityEvent weakShake;
    public UnityEvent mediumShake;
    public UnityEvent strongShake;

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

            if (shakeForce > strongShakeForce)
            {
                strongShake.Invoke();
                Debug.Log("Strong Shake");
                return;
            }
            if (shakeForce > mediumShakeForce)
            {
                mediumShake.Invoke();
                Debug.Log("Medium Shake");
                return;
            }
            if (shakeForce > weakShakeForce)
            {
                weakShake.Invoke();
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
