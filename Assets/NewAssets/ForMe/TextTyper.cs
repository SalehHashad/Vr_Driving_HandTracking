using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{
    public Text introText;
    public AudioSource typingSoundSource;
    public AudioClip[] typingSounds;
    public string[] messages;
    public float typingSpeed = 0.05f;
    public float delayBetweenMessages = 2f;

    private int currentMessageIndex = 0;


    void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (string message in messages)
        {
            introText.text = "";
            AudioClip currentTypingSound = typingSounds[currentMessageIndex];

            foreach (char letter in message.ToCharArray())
            {
                introText.text += letter;
                if (typingSoundSource && currentTypingSound)
                {
                    typingSoundSource.clip = currentTypingSound;
                    typingSoundSource.Play();
                }
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(delayBetweenMessages);

            currentMessageIndex++;
        }
        gameObject.SetActive(false);
    }
}
