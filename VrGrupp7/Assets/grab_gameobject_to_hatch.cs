using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class grab_gameobject_to_hatch : MonoBehaviour
{
    Hatch hatch;
    public Hatch findHatch;
    JobManager jobManager;
    public GameObject findjobManager;

    public GameObject target_of_submitted_object;
    public GameObject target_wrong_throw_it_out;
    Color color0 = Color.red;
    Color color1 = Color.green;
    public Light lt;
    bool is_object_under_hatch = true;

    float conveyor_belt_speed = 0.01f;
    void Start()
    {
        hatch = findHatch.GetComponent<Hatch>();
        jobManager = findjobManager.GetComponent<JobManager>();
    }

    private void OnTriggerStay(Collider other) 
    {
        if (hatch.rotate == false && jobManager.turn_in_correct == 1)
        {
            lt.color = (color1);
            movetowards_hatch();
           
            
            if (Vector3.Distance(other.gameObject.transform.position, target_of_submitted_object.transform.position) < 0.1f)
            {
                    Destroy(other.gameObject);
            }
        }
        if (hatch.rotate == false && jobManager.turn_in_correct == 2)//is NOT correct
            {
                if (Vector3.Distance(other.gameObject.transform.position, target_of_submitted_object.transform.position) > 0.1f && is_object_under_hatch == true)
                    {
                    Debug.Log("go inside");
                    movetowards_hatch();
                    }

                if (Vector3.Distance(other.gameObject.transform.position, target_of_submitted_object.transform.position) <= 0.5f)
                    {
                    lt.color = (color0);
                    is_object_under_hatch = false;
                    Debug.Log("go OUT");
                    movetowards_target_throw_it_out();
                    }       
            }

            void movetowards_hatch()
            {
                other.gameObject.transform.position = Vector3.MoveTowards(other.gameObject.transform.position,
                target_of_submitted_object.transform.position, conveyor_belt_speed);
            }
            void movetowards_target_throw_it_out()
            {
                other.gameObject.transform.position = Vector3.MoveTowards(other.gameObject.transform.position,
                target_of_submitted_object.transform.position, conveyor_belt_speed);
            }
    }
}
