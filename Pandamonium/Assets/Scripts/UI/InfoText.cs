using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour {

    public Color color;

    private Text text;

    Color endColor;

    Stack<string> messagesToShow = new Stack<string>();

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

        ShowMessage("Level " + (GameManager.I.currentLevel + 1).ToString());

	}
	
    public void ShowMessage(string message)
    {
        messagesToShow.Push(message);

        if(!text.enabled && messagesToShow.Count == 1)
        {
            ShowMessageFromStack();
        }
    }

    private void ShowMessageFromStack()
    {
        if (messagesToShow.Count == 0)
            return;

        StartCoroutine(Lifetime(messagesToShow.Pop()));
    }

	private IEnumerator Lifetime(string message)
    {
        text.enabled = true;
        text.text = message;

        text.color = new Color(color.r, color.g, color.b, 0);

        float i = 0;

        while (i < 0.25f)
        {
            i += Time.deltaTime;
            text.color = Color.Lerp(text.color, color, i * 4);
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
