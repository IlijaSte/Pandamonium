using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuest : Quest {

    public int coinsToCollect = 100;

    private int coinsCollected = 0;

    protected override void Start()
    {
        base.Start();
        type = QuestType.COLLECT;
    }

    private void OnCollected()
    {
        progress++;

        if(progress >= goal)
        {
            (GameManager.I.playerInstance as PlayerWithJoystick).onCoinCollected -= OnCollected;
            OnCompleted();
        }
    }

    public override void SetupDelegates()
    {
        base.SetupDelegates();

        (GameManager.I.playerInstance as PlayerWithJoystick).onCoinCollected += OnCollected;
    }

    public override string GetParameter()
    {
        return "";
    }

    public override void SetParameter(string param)
    {
        
    }
}
