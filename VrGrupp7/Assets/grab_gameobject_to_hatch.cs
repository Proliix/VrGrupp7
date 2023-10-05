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

//___________________________
    public GameObject gameobject_hatch;
//___________________________

    public GameObject target_of_submitted_object;
    public GameObject target_wrong_throw_it_out;
//___________________________
    Color color0 = Color.red;
    Color color1 = Color.green;
    Color color2 = Color.yellow;
    public Light lt;
//___________________________
    bool is_object_under_hatch = true;

    float conveyor_belt_speed = 0.01f;
//___________________________
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
                hatch.rotate = true;
                jobManager.turn_in_correct = 0;
                Debug.Log(jobManager.turn_in_correct);
                
                if (hatch.rotate == true)
                {
                Debug.Log(hatch.rotationAngle);
                hatch.rotationAngle += hatch.rotationSpeed;
                gameobject_hatch.transform.Rotate(new Vector3(0,0, -hatch.rotationAngle) * Time.deltaTime, Space.Self);   
                }

                if(hatch.rotationAngle > 190.0f && jobManager.turn_in_correct == 0)
                {
                hatch.rotate = false;
                }
                lt.color = (color2);
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
