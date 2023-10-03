using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Shake : MonoBehaviour
{
    public float minimumShakePercentage = 5;
    public float shakeForceModifier = 1;
    public UnityEvent<float> onShake;

    private Rigidbody rb;
    private Vector3 oldPosition;

    //public float weakShakeForce = 5;
    //public float mediumShakeForce = 15;
    //public float strongShakeForce = 25;

    //public UnityEvent weakShake;
    //public UnityEvent mediumShake;
    //public UnityEvent strongShake;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oldPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!rb.IsSleeping())
        {
            float shakePercentage = GetShakeForce() / 100f;
            oldPosition = rb.position;

            if(shakePercentage > minimumShakePercentage / 100f)
            {
                Debug.Log("Force: " + shakePercentage);
                onShake.Invoke(shakePercentage);
            }

            //Debug.Log(shakeForce);

            //if (shakeForce > strongShakeForce)
            //{
            //    strongShake.Invoke();
            //    //Debug.Log("Strong Shake");
            //    return;
            //}
            //if (shakeForce > mediumShakeForce)
            //{
            //    mediumShake.Invoke();
            //    //Debug.Log("Medium Shake");
            //    return;
            //}
            //if (shakeForce > weakShakeForce)
            //{
            //    weakShake.Invoke();
            //    //Debug.Log("Weak Shake");
            //    return;
            //}
        }
    }


    //Can be optimized with magnitude squared
    float GetShakeForce()
    {
        return ((rb.position - oldPosition).magnitude * shakeForceModifier ) / Time.deltaTime;
    }
}
