using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public int maxNumOfDailies = 3;

    public List<QuestInfo> questPool;

    private List<Quest> activeQuests = new List<Quest>();

    private static QuestManager instance;

    public static QuestManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestManager>();
            }
            return instance;
        }
    }

    [Serializable]
    public class QuestInfo
    {
        public Quest.QuestType type;
        public string parameter;
        public int goal;
        public int progress;
        public int rewardCoins;

        public QuestInfo(Quest.QuestType type, string param, int goal, int progress, int rewardCoins)
        {
            this.type = type;
            parameter = param;
            this.goal = goal;
            this.progress = progress;
            this.rewardCoins = rewardCoins;
        }
    }

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void SetupPlayerDelegates()
    {
        foreach(Quest q in GetComponents<Quest>())
        {
            q.SetupDelegates();
        }
    }

    public void ResetQuests()
    {
        LoadQuestInfos(null);
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    public List<QuestInfo> GetQuestInfos()
    {
        List<QuestInfo> infos = new List<QuestInfo>();

        foreach(Quest q in activeQuests)
        {
            
            infos.Add(new QuestInfo(q.type, q.GetParameter(), (q.loadedInfo ? q.goal : q.goalTemp), (q.loadedInfo ? q.progress : q.progressTemp), q.rewardCoins));
        }

        return infos;

    }

    protected void OnQuestCompleted(Quest q)
    {
        print(activeQuests.Remove(q));
        Destroy(q);
        SaveManager.I.SaveGame();
    }

    public void LoadQuestInfos(List<QuestInfo> infos)
    {
        if (infos == null || infos.Count == 0)
        {

            foreach (Quest q in GetComponents<Quest>())
            {
                Destroy(q);
            }

            activeQuests.Clear();

            List<QuestInfo> usedQuests = new List<QuestInfo>();

            for(int i = 0; i < maxNumOfDailies; i++)
            {

                QuestInfo newQ;

                do {
                    newQ = questPool[UnityEngine.Random.Range(0, questPool.Count)];
                } while (usedQuests.Contains(newQ));

                Quest newQuest = null;

                switch (newQ.type)
                {
                    case Quest.QuestType.COLLECT:
                        newQuest = gameObject.AddComponent<CollectQuest>();
                        break;

                    case Quest.QuestType.SLAY:
                        newQuest = gameObject.AddComponent<SlayQuest>();
                        break;
                }

                newQuest.goalTemp = newQ.goal;
                newQuest.progressTemp = newQ.progress;

                newQuest.SetParameter(newQ.parameter);

                newQuest.rewardCoins = newQ.rewardCoins;

                newQuest.type = newQ.type;

                newQuest.onCompleted += OnQuestCompleted;

                usedQuests.Add(newQ);
                activeQuests.Add(newQuest);
            }

        }
        else if(GetComponents<Quest>().Length == 0)
        {

            foreach(Quest q in GetComponents<Quest>())
            {
                Destroy(q);
            }

            activeQuests.Clear();

            foreach (QuestInfo qInfo in infos)
            {
                Quest newQuest = null;
                switch (qInfo.type)
                {
                    case Quest.QuestType.COLLECT:
                        newQuest = gameObject.AddComponent<CollectQuest>();

                        break;

                    case Quest.QuestType.SLAY:
                        newQuest = gameObject.AddComponent<SlayQuest>();
                        newQuest.SetParameter(qInfo.parameter);
                        break;
                }

                newQuest.progressTemp = qInfo.progress;
                newQuest.goalTemp = qInfo.goal;
                newQuest.rewardCoins = qInfo.rewardCoins;
                newQuest.type = qInfo.type;
                newQuest.onCompleted += OnQuestCompleted;
                activeQuests.Add(newQuest);
            }
        }
    }
}
