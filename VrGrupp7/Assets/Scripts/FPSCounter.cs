using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsObj;
    [SerializeField] TextMeshProUGUI avrageObj;
    [SerializeField] float updateTime = 2f;

    int fps;

    float timer;

    // Update is called once per frame
    void Update()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        timer += Time.deltaTime;

        if (timer >= updateTime)
        {
            fpsObj.text = "<align=justified>" + "FPS: " + fps + "</align>";
            avrageObj.text = "<align=justified>" + "Avrage fps: " + (int)(Time.frameCount / Time.time) + "</align>";
            timer -= updateTime;
        }
    }
}
