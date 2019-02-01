using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestText : MonoBehaviour {

    public Text text;
    public Text questText;
    public Text progressText;
    public Slider slider;

    CanvasGroup canvasGroup;

    Queue<QuestMessage> messagesToShow = new Queue<QuestMessage>();

    private static QuestText instance;

    private struct QuestMessage
    {
        public string text;
        public int progress;
        public int goal;

        public QuestMessage(string text, int progress, int goal)
        {
            this.text = text;
            this.progress = progress;
            this.goal = goal;

        }
    }

    public static QuestText I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestText>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

    }

    public void ShowMessage(string message, int progress, int goal)
    {

        QuestMessage msg = new QuestMessage(message, progress, goal);

        messagesToShow.Enqueue(msg);

        if (canvasGroup.alpha == 0 && messagesToShow.Count == 1)
        {
            ShowMessageFromStack();
        }
    }

    private void ShowMessageFromStack()
    {
        if (messagesToShow.Count == 0)
            return;

        QuestMessage msg = messagesToShow.Dequeue();

        StartCoroutine(Lifetime(msg.text, msg.progress, msg.goal));
    }

    private IEnumerator Lifetime(string message, int progress, int goal)
    {
        //canvasGroup.alpha = 0;
        questText.text = message;
        slider.gameObject.SetActive(true);
        progressText.enabled = true;
        slider.value = progress / (float)goal;
        
        if(slider.value == 1)
        {
            text.text = "quest complete!";
            slider.gameObject.SetActive(false);
            progressText.enabled = false;
        }
        else
        {
            text.text = "quest:";
            progressText.text = progress.ToString() + " / " + goal.ToString();
        }

        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, i);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        i = 0;

        while (i < 1)
        {
            i += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, i);
            yield return null;
        }

        canvasGroup.alpha = 0;

        ShowMessageFromStack();
    }
}
