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
    public bool rotate = true;

    float timer;
    void Start()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
    }

    void Update()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
        
        if (jobManager.turn_in_correct == 1 || jobManager.turn_in_correct == 2)
        {
            if (rotate == true)
            {
            rotationAngle += rotationSpeed;
            gameObject.transform.Rotate(new Vector3(0,0, -rotationAngle) * Time.deltaTime, Space.Self);   
            }

            if(rotationAngle > 190.0f)
            {
            rotate = false;
            }
        }
    }
}