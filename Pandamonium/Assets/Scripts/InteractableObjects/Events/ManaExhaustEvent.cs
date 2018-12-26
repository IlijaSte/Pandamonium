using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaExhaustEvent : ShrineEvent {

    public float exhaustPercent = 100;

    protected override void Activate()
    {
        base.Activate();

        Complete();
    }

    protected override void Complete()
    {
        base.Complete();
        (GameManager.I.playerInstance as PlayerWithJoystick).DecreaseEnergy(exhaustPercent, true);

        InfoText.I.ShowFailedMessage("mana exhausted");
    }
}
