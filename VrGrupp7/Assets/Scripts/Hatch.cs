using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    [SerializeField] JobManager jobManager;
    [SerializeField] Transform potionHolderPos;
    [SerializeField] GameObject potion;
    [SerializeField] GameObject arm;
    [SerializeField] Animator anim;
    [SerializeField] AudioClip correctSound;
    [SerializeField] AudioClip failSound;
    public float rotationSpeed = 1f;
    public float rotationAngle;
    public bool rotate = true;

    bool correctPotion;
    float timer;
    Animator armAnim;
    void Start()
    {
        armAnim = arm.GetComponent<Animator>();
    }

    public void StartSequence(GameObject bottle, bool isCorrect)
    {
        correctPotion = isCorrect;
        potion = bottle;
        OpenHatch();
        StartArm();
    }

    #region Animation Functions
    void StartArm()
    {
        armAnim.SetTrigger("Start");
    }

    void OpenHatch()
    {
        anim.SetTrigger("Open");
    }

    public void CloseHatch()
    {
        anim.SetTrigger("Close");
    }

    public void AttachPotion()
    {
        if (potion != null)
        {
            potion.transform.position = potionHolderPos.transform.position;
            potion.transform.parent = potionHolderPos;
        }
        else
            Debug.LogWarning("Could not find bottle");
    }

    public void DeAttachPotion()
    {
        potion.transform.parent = null;
    }
    #endregion

    public void CheckPotion()
    {
        if (correctPotion)
        {
            AudioSource.PlayClipAtPoint(correctSound, anim.gameObject.transform.position);
            jobManager.TurnInCorrect();
            Destroy(potion);
            jobManager.ResetTurnIn();
        }
        else
        {
            AudioSource.PlayClipAtPoint(failSound, anim.gameObject.transform.position);
            jobManager.TurnInIncorrect();
            StartCoroutine(ThrowAwayPotion());
        }
    }

    public IEnumerator ThrowAwayPotion()
    {
        OpenHatch();
        DeAttachPotion();
        yield return new WaitForSeconds(1f);
        Rigidbody rbd = potion.GetComponent<Rigidbody>();
        rbd.isKinematic = false;
        rbd.AddForce((anim.gameObject.transform.position - potion.transform.position) * Random.Range(4f, 8f), ForceMode.Impulse);
        yield return new WaitForSeconds(.5f);
        CloseHatch();
        yield return new WaitForSeconds(1f);
        jobManager.ResetTurnIn();
    }

    //void Update()
    //{
    //    if (jobManager == null)
    //        jobManager = findjobManager.GetComponent<JobManager>();



    //    if (jobManager.turn_in_correct == 1 || jobManager.turn_in_correct == 2)
    //    {
    //        if (rotate == true)
    //        {
    //            rotationAngle += rotationSpeed;
    //            gameObject.transform.Rotate(new Vector3(0, 0, -rotationAngle) * Time.deltaTime, Space.Self);
    //        }

    //        if (rotationAngle > 190.0f && jobManager.turn_in_correct == 1 || jobManager.turn_in_correct == 2)
    //        {
    //            Debug.Log("I WANT IT TO BE FALSE");
    //            rotate = false;
    //        }
    //    }
    //}
}