using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour {

    public Color color;

    private Text text;

    Color endColor;

    private struct InfoMessage
    {
        public string message;
        public Color color;

        public InfoMessage(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }
    }

    Queue<InfoMessage> messagesToShow = new Queue<InfoMessage>();

    private static InfoText instance;

    public static InfoText I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InfoText>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start () {

        text = GetComponent<Text>();

        ShowMessage("level " + (GameManager.I.currentLevel + 1).ToString());

	}
	
    public void ShowMessage(string message)
    {
        InfoMessage newMessage = new InfoMessage(message, color);
        messagesToShow.Enqueue(newMessage);

        if(!text.enabled && messagesToShow.Count == 1)
        {
            ShowMessageFromStack();
        }
    }

    public void ShowFailedMessage(string message)
    {
        InfoMessage newMessage = new InfoMessage(message, Color.red);
        messagesToShow.Enqueue(newMessage);

        if (!text.enabled && messagesToShow.Count == 1)
        {
            ShowMessageFromStack();
        }
    }

    private void ShowMessageFromStack()
    {
        if (messagesToShow.Count == 0)
            return;

        StartCoroutine(Lifetime(messagesToShow.Dequeue()));
    }

	private IEnumerator Lifetime(InfoMessage message)
    {
        text.enabled = true;
        text.text = message.message;

        text.color = new Color(message.color.r, message.color.g, message.color.b, 0);

        float i = 0;

        while (i < 0.25f)
        {
            i += Time.deltaTime;
            text.color = Color.Lerp(text.color, message.color, i * 4);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        endColor = new Color(text.color.r, text.color.g, text.color.b, 0);
        i = 0;

        while(i < 1)
        {
            i += Time.deltaTime; 
            text.color = Color.Lerp(text.color, endColor, i);
            yield return null;
        }

        text.enabled = false;// Destroy(gameObject);

        ShowMessageFromStack();
    }
}
