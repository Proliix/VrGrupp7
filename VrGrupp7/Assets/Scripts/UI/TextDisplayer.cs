using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplayer : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI tmpDisplayText;
    [SerializeField] private TextMeshProUGUI tmpMessageCounter;
    [SerializeField][Range(0.5f, 2)] private float textSpeed = 1;

    bool isWritingText = false;
    bool skipText = false;

    public string[] messages;
    int messageIndex = 0;

    public string displayText = "";

    // Start is called before the first frame update
    void Start()
    {
        var temp = Resources.Load<TextAsset>("Tutorial");
        messages = temp.text.Split('\n');

        WriteText();
    }

    // Update is called once per frame
    void Update()
    {
        tmpDisplayText.text = displayText;
    }

    public void Next()
    {
        if (isWritingText) 
        {
            skipText = true;
            return; 
        }

        if (messageIndex >= messages.Length - 1)
        {
            Debug.Log("Can't write message, displaying last message");
            return;
        }

        messageIndex++;
        WriteText();
    }

    public void Previous()
    {
        if (isWritingText)
        {
            skipText = true;
            return;
        }

        if (messageIndex <= 0)
        {
            Debug.Log("Can't write prev message, at 0");
            return;
        }

        messageIndex--;
        WriteText();
    }

    private void WriteText()
    {
        if (isWritingText) { return; }

        isWritingText = true;
        UpdateCounterText();

        StartCoroutine(IE_WriteText(messages[messageIndex], textSpeed));
    }

    private void UpdateCounterText()
    {
        tmpMessageCounter.text = "<mspace=0.8em>"+ (messageIndex + 1) + "/" + messages.Length + "</mspace>";
    }

    private IEnumerator IE_WriteText(string text, float textSpeed)
    {
        int index = 1;

        while (text.Length > index && !skipText)
        {
            displayText = text.Substring(0, index);
            yield return new WaitForSeconds(0.1f / textSpeed);
            index++;
        }

        displayText = text;
        skipText = false;
        isWritingText = false;
    }
}
