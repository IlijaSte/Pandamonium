using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayQuest : Quest {

    public string enemyName;

    protected override void Start()
    {
        base.Start();
        type = QuestType.SLAY;
    }

    public override void SetupDelegates()
    {
        base.SetupDelegates();

        (GameManager.I.playerInstance as PlayerWithJoystick).onEnemySlain += OnEnemySlain;
    }

    void OnEnemySlain(string enemy)
    {
        if (enemy.Equals(enemyName))
        {
            progress++;
            QuestText.I.ShowMessage("slay " + goal.ToString() + " " + enemyName + "s", progress, goal);
            if (progress == goal)
            {
                (GameManager.I.playerInstance as PlayerWithJoystick).onEnemySlain -= OnEnemySlain;
                OnCompleted();
            }
            else
            {
                //QuestText.I.ShowMessage("Slay " + goal.ToString() + " " + enemyName + "s", progress, goal);
            }
        }
    }

    public override string GetParameter()
    {
        return enemyName;
    }

    public override void SetParameter(string param)
    {
        enemyName = param;
    }
}
