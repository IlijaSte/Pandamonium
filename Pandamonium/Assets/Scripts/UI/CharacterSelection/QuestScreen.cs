using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestScreen : MonoBehaviour {

    public Canvas questCanvas;

    public Transform questList;

    public GameObject questViewPrefab;

    private void Start()
    {
        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        List<QuestManager.QuestInfo> quests = QuestManager.I.GetQuestInfos();

        foreach (QuestManager.QuestInfo q in quests)
        {
            string qText = "";
            switch (q.type)
            {
                case Quest.QuestType.COLLECT:
                    qText = "Collect " + q.goal.ToString() + " coins";
                    break;

                case Quest.QuestType.SLAY:
                    qText = "Slay " + q.goal.ToString() + " " + q.parameter + "s";
                    break;
            }

            Instantiate(questViewPrefab, questList).GetComponent<QuestView>().InitTexts(qText, q.goal, q.progress, q.rewardCoins);
        }
    }

    public void ResetQuests()
    {
        QuestManager.I.ResetQuests();

        foreach (Transform child in questList)
        {
            Destroy(child.gameObject);
        }

        UpdateQuestList();
    }

    public void ChangeScreen(Toggle toggle)
    {
        questCanvas.enabled = toggle.isOn;
    }

}
