using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryBoxManager : MonoBehaviour {

    public float textSpeed = 1.5f;

    private string[] stories;

    public RectTransform storyPanel;
    public Button nextButton;

    public Text text;

    public float endY;

    [HideInInspector]
    public bool isShown = false;

    private Canvas canvas;

    private int currStory = 0;

    Color endColor;

    Queue<string> messagesToShow = new Queue<string>();

    private static StoryBoxManager instance;

    private float panelSpeed = 1;

    private float defaultY;

    private Coroutine activeCoroutine;

    public static StoryBoxManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoryBoxManager>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        canvas = GetComponent<Canvas>();
        defaultY = storyPanel.anchoredPosition.y;
    }

    private void ShowCanvas()
    {
        canvas.enabled = true;
    }

    private void HideCanvas()
    {
        canvas.enabled = false;
    }

    public void WriteNextStory()
    {
        if (!isShown)
        {
            return;
        }

        if (currStory < stories.Length)
        {
            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);
            activeCoroutine = StartCoroutine(DoWrite(stories[currStory++]));
            if(currStory >= stories.Length)
            {
                //nextButton.GetComponentInChildren<Text>().text = "close";
            }
        }
        else
        {
            //Time.timeScale = 1;
            (GameManager.I.playerInstance as PlayerWithJoystick).canMove = true;

            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);

            activeCoroutine = StartCoroutine(HidePanel());
            nextButton.interactable = false;
            UIManager.I.ShowUI();
        }
    }

    public void ShowStory(string[] stories)
    {

        if (isShown)
            return;

        this.stories = stories;
        currStory = 0;

        //Time.timeScale = 0;
        (GameManager.I.playerInstance as PlayerWithJoystick).canMove = false;
        nextButton.interactable = false;
        //nextButton.GetComponentInChildren<Text>().text = "next";

        isShown = true;

        ShowCanvas();
        StartCoroutine(ShowPanel());

        UIManager.I.HideUI();
    }

    private IEnumerator DoWrite(string txt)
    {
        text.text = "";

        for (int j = 0; j < txt.Length; j++)
        {
            text.text += txt[j];
            yield return new WaitForSeconds(1 / (textSpeed * 50));
        }
    }

    private IEnumerator HidePanel()
    {
        
        Vector2 startPos = storyPanel.anchoredPosition;
        Vector2 endPos = new Vector2(storyPanel.anchoredPosition.x, defaultY);

        float i = 0;

        while (i < 1f)
        {
            i += Time.deltaTime * panelSpeed;
            //text.color = Color.Lerp(text.color, color, i * 4);
            storyPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, i));

            yield return null;
        }

        HideCanvas();
        isShown = false;
    }

    private IEnumerator ShowPanel()
    {

        text.text = "";

        Vector2 startPos = storyPanel.anchoredPosition;
        Vector2 endPos = new Vector2(storyPanel.anchoredPosition.x, endY);
        
        float i = 0;

        while (i < 1f)
        {
            i += Time.deltaTime * panelSpeed;
            //text.color = Color.Lerp(text.color, color, i * 4);
            storyPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, i));

            yield return null;
        }

        nextButton.interactable = true;
        WriteNextStory();

    }
}
