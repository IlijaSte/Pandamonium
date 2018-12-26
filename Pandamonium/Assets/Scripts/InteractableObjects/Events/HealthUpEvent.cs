using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpEvent : ShrineEvent {

    public float healPercent = 20;

    protected override void Activate()
    {
        base.Activate();

        Complete();
    }

    protected override void Complete()
    {
        base.Complete();

        InfoText.I.ShowMessage("health up");
        (GameManager.I.playerInstance as PlayerWithJoystick).Heal(healPercent, true);
    }

}
