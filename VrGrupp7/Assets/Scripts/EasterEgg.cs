using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    [SerializeField] AudioSource DoorSource;
    [SerializeField] AudioSource SourceButton;
    [SerializeField] AudioClip denyClip;
    [SerializeField] AudioClip acceptClip;

    Animator anim;
    float timesPressed;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ButtonPress()
    {
        timesPressed++;
        if (timesPressed == 5)
        {
            SourceButton.PlayOneShot(acceptClip);
            anim.SetTrigger("Start");
            DoorSource.Play();
        }
        else if (timesPressed < 5)
            SourceButton.PlayOneShot(denyClip);

    }

    public void ResetAnim()
    {
        timesPressed = 0;
    }

}
