using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsArm : MonoBehaviour
{
    [SerializeField] Hatch hatch;

    public void GrabPotion()
    {
        hatch.AttachPotion();
    }

    public void CloseHatch()
    {
        hatch.CloseHatch();
    }

    public void AnimationDone()
    {
        hatch.CheckPotion();
    }

}
