using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pestle : MonoBehaviour
{
    Rigidbody rb;
    Collider _collider;

    Vector3 oldPosition;
    Vector3 velocity;

    [SerializeField]float grindDamage = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    void Update()
    {
        if (!rb.IsSleeping())
        {
            velocity = (transform.position - oldPosition);
            oldPosition = transform.position;
        }
    }

    public void SetTrigger(bool isTrigger)
    {
        _collider.isTrigger = isTrigger;
    }

    private float GetEfficieny(Vector3 mortarCenter)
    {
        if (this.velocity == Vector3.zero)
            return 0;

        Vector2 pestleLocation = new Vector2(transform.position.x, transform.position.z);
        Vector2 mortarLocation = new Vector2(mortarCenter.x, mortarCenter.z);

        Vector2 velocity = new Vector2(this.velocity.x, this.velocity.z);

        Vector2 direction = pestleLocation - mortarLocation;
        Vector2 normal = Vector2.Perpendicular(direction);

        //Debug.Log("Direction: " + direction);
        //Debug.Log("Normal: " + normal);
        //Debug.Log("Velocity: " + velocity);

        //Debug.DrawLine(mortarCenter, mortarCenter + new Vector3(direction.x, 0, direction.y), Color.red);
        //Debug.DrawLine(transform.position, transform.position + new Vector3(normal.x, 0, normal.y), Color.blue);

        float angle = Vector2.Angle(normal, velocity);

        if (angle > 90)
            angle = 180 - angle;

        float efficiency01 = 1 - angle / 90f;

        //Debug.Log("efficiency01: " + efficiency01);
        return efficiency01;
    }

    public float GetDamage(Vector3 mortarCenter)
    {
        float efficiency01 = GetEfficieny(mortarCenter);

        float damage = grindDamage * efficiency01 * velocity.magnitude;

        return damage;
    }
}
