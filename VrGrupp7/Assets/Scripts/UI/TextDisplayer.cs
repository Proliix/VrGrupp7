using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplayer : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI tmpDisplayText;
    [SerializeField] private TextMeshProUGUI tmpMessageCounter;
    [SerializeField] [Range(0.5f, 2)] private float textSpeed = 1;

    [SerializeField] private AudioSource typeAudio;
    [SerializeField] private AudioClip typeSound;

    [SerializeField] private AudioSource buttonAudio;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private bool isTutorial;

    bool isWritingText = false;
    bool skipText = false;

    public string[] messages;
    int messageIndex = 0;

    void Start()
    {
        if (isTutorial)
        {
            var temp = Resources.Load<TextAsset>("Tutorial");
            messages = temp.text.Split('\n');

            WriteText();
        }
    }

    public void Next()
    {
        buttonAudio.PlayOneShot(buttonSound);

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

        buttonAudio.PlayOneShot(buttonSound);

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

    public void WriteText(string Text)
    {
        if (isWritingText) { return; }

        isWritingText = true;

        StartCoroutine(IE_WriteText(Text, textSpeed));
    }

    public void ForceWriteText(string Text)
    {
        if (isWritingText) { StopAllCoroutines(); }

        isWritingText = true;

        StartCoroutine(IE_WriteText(Text, textSpeed));
    }

    private void UpdateCounterText()
    {
        tmpMessageCounter.text = "<mspace=0.8em>" + (messageIndex + 1) + "/" + messages.Length + "</mspace>";
    }

    private IEnumerator IE_WriteText(string text, float textSpeed)
    {
        int index = 1;
        tmpDisplayText.maxVisibleCharacters = 0;
        tmpDisplayText.text = text;
        typeAudio.clip = typeSound;

        float offset = 0.2f;
        char[] charArr = text.ToCharArray();
        bool skipMarkdown = false;
        int markdownChars = 0;

        while (text.Length > index && !skipText)
        {
            if (charArr[index] == '<')
                skipMarkdown = true;

            if (!skipMarkdown)
            {
                if (index % 2 == 0)
                {
                    typeAudio.pitch = 1 + Random.Range(-offset, offset);
                    typeAudio.volume = (1 - offset) + Random.Range(-offset, offset);
                    //audioSource.PlayOneShot(typeSound);
                    typeAudio.Play();
                }

                yield return new WaitForSeconds(0.1f / textSpeed);
                //tmpDisplayText.text = text.Substring(0, index);
                tmpDisplayText.maxVisibleCharacters = index - markdownChars;
                tmpDisplayText.ForceMeshUpdate();
            }
            else
                markdownChars++;

            if (charArr[index] == '>')
                skipMarkdown = false;

            index++;
        }

        //tmpDisplayText.text = text;
        tmpDisplayText.maxVisibleCharacters = text.Length;
        skipText = false;
        isWritingText = false;
    }
}
