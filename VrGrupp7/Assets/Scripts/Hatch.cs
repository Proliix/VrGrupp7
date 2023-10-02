using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    JobManager jobManager;
    public GameObject findjobManager;
    float _movespeed = 10f;
    void Start()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
        Debug.Log(jobManager.turn_in_correct);
    }

    void Update()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
        if (jobManager.turn_in_correct == 0)
        {
            gameObject.transform.Rotate(new Vector3(0,0, -90f) * Time.deltaTime, Space.Self);
        }

        if (jobManager.turn_in_correct == 2)
        {
            gameObject.transform.Rotate(new Vector3(0,0, -90) * Time.deltaTime, Space.Self);
        }
    }
}
