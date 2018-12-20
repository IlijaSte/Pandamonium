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

            if(progress == goal)
            {
                (GameManager.I.playerInstance as PlayerWithJoystick).onEnemySlain -= OnEnemySlain;
                OnCompleted();
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
