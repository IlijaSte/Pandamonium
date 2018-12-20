using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour {

    public Text questText;
    public Text progressText;
    public Text rewardText;

    public void InitTexts(string text, int goal, int progress, int coins)
    {
        questText.text = text;
        progressText.text = progress.ToString() + " / " + goal.ToString();
        rewardText.text = coins.ToString();
    }
}
