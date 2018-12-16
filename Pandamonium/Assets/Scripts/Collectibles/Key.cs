using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Collectible {

    protected override void OnPickup()
    {
        (GameManager.I.playerInstance as PlayerWithJoystick).PickupKey();
        base.OnPickup();
    }

}
