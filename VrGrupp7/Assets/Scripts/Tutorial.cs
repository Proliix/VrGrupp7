using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Tutorial : MonoBehaviour
{
    public InputActionReference leftStick;
    public InputActionReference rightStick;

    public AudioClip sucessClip;
    public GameObject screen;
    public GameObject bottle;
    public GameObject platform;
    public XRSocketInteractor socket;
    public Image fillImage;

    TextDisplayer displayer;
    float movedAmount;

    Transform headTransform;
    bool moveComplete;
    bool snapComplete;
    bool completeText;
    bool hasBeenGrabbed;

    bool loading;

    private void Start()
    {
        displayer = screen.GetComponent<TextDisplayer>();
        displayer.WriteText("Use the left stick to move around! Try it out!");
        fillImage.fillAmount = 0;
        headTransform = Camera.main.transform;
    }

    private void CheckActionAxis(InputActionReference input, ref bool boolToChange, Vector2 minValues, Vector2 maxValues, string textToChangeTo)
    {
        Vector2 value = input.action.ReadValue<Vector2>();
        if (value.x > maxValues.x || value.x < minValues.x || value.y > maxValues.y || value.y < minValues.y)
        {
            movedAmount += 0.33f * Time.deltaTime;
            fillImage.fillAmount = movedAmount;
        }


        if (movedAmount >= 1)
        {
            boolToChange = true;
            AudioSource.PlayClipAtPoint(sucessClip, displayer.transform.position);
            displayer.ForceWriteText(textToChangeTo);
            movedAmount = 0;
            fillImage.fillAmount = 0;
        }
    }

    public void BottleIsGrabbed(SelectExitEventArgs args)
    {
        hasBeenGrabbed = true;
        socket.selectExited.RemoveListener(BottleIsGrabbed);

        AudioSource.PlayClipAtPoint(sucessClip, displayer.transform.position);
        platform.SetActive(false);
        displayer.ForceWriteText("Perfect! Now i want you to <size=20><color=red><B>SMASH IT!</B></color></size>");
    }

    private void Update()
    {
        screen.transform.rotation = Quaternion.LookRotation(screen.transform.position - headTransform.position);


        if (!moveComplete)
        {
            CheckActionAxis(leftStick, ref moveComplete, new Vector2(-0.1f, -0.1f), new Vector2(0.1f, 0.1f), "Well done! Now try to snap turn by using your right stick to the left,right or down");
            return;
        }

        if (!snapComplete)
        {
            CheckActionAxis(rightStick, ref snapComplete, new Vector2(-0.1f, -0.1f), new Vector2(0.1f, 20f), "Great! Now use the buttons on the back of the controller to grab the bottle");
            return;
        }
        else if (platform.activeSelf == false && !hasBeenGrabbed)
        {

            platform.SetActive(true);
            bottle.SetActive(true);
            socket.selectExited.AddListener(BottleIsGrabbed);
        }

        if (bottle == null && hasBeenGrabbed)
        {
            if (!completeText)
            {
                AudioSource.PlayClipAtPoint(sucessClip, displayer.transform.position);
                displayer.ForceWriteText("Well done! Lets take you to the lab so you make some potions!");
                completeText = true;
            }



            movedAmount += 0.15f * Time.deltaTime;
            fillImage.fillAmount = movedAmount;

            if (!loading && movedAmount >= 1)
            {

                loading = true;
                SceneManager.LoadScene(1);
            }

        }

    }

}
