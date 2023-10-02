using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    JobManager jobManager;
    public GameObject findjobManager;
    void Start()
    {
        jobManager = findjobManager.GetComponent<JobManager>();
        Debug.Log(jobManager.turn_in_correct);
    }

    void Update()
    {
        if (jobManager.turn_in_correct == 1)
        {
            
        }

        if (jobManager.turn_in_correct == 2)
        {
            
        }
    }
}
