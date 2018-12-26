using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRechargeEvent : ShrineEvent {

    protected override void Activate()
    {
        base.Activate();

        Complete();
    }

    protected override void Complete()
    {
        base.Complete();
        (GameManager.I.playerInstance as PlayerWithJoystick).IncreaseEnergy(100, true);

        InfoText.I.ShowMessage("mana up");
    }
}
