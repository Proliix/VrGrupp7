using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    JobManager jobManager;
    public GameObject findjobManager;
    float rotationSpeed = 1f;

    float rotationAngle;
    bool rotate = true;

    float timer;
    void Start()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
        //Debug.Log(jobManager.turn_in_correct);
    }

    void Update()
    {
        
        jobManager = findjobManager.GetComponent<JobManager>();
        
        Debug.Log(timer);
        if (jobManager.turn_in_correct == 0)
        {
            if (rotate == true)
            {
            rotationAngle += rotationSpeed;
            //transform.Rotate(0, 0, -rotationAngle);
            gameObject.transform.Rotate(new Vector3(0,0, -rotationAngle) * Time.deltaTime, Space.Self);
                
            }

            if(rotationAngle > 180.0f)
            {
            rotate = false;
            }

            //gameObject.transform.Rotate(new Vector3(0,0, -90f) * Time.deltaTime, Space.Self);
            //gameObject.transform.Rotate(new Vector3(0,0, 0) * Time.deltaTime, Space.Self);
        }

        if (jobManager.turn_in_correct == 2 )
        {
            gameObject.transform.Rotate(new Vector3(0,0, -90) * Time.deltaTime, Space.Self);
            gameObject.transform.Rotate(new Vector3(0,0, 0) * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("tigger?");
        other.gameObject.transform.Translate(new Vector3(0,0, 50) * Time.deltaTime, Space.Self);
    }
}