using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaExhaustEvent : ShrineEvent {

    public float exhaustPercent = 100;

    protected override void Activate()
    {
        base.Activate();

        (GameManager.I.playerInstance as PlayerWithJoystick).DecreaseEnergy(exhaustPercent, true);
    }
}
