using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    [SerializeField] AudioSource SourceButton;
    [SerializeField] AudioClip lightClip;

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
            anim.SetTrigger("Start");
        }
    }

    public void ResetAnim()
    {
        timesPressed = 0;
    }

}
