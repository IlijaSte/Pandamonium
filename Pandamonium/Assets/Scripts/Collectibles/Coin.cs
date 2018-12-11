using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectible {

	protected override void OnPickup()
    {

        (GameManager.I.playerInstance as PlayerWithJoystick).PickupCoins(1);
        base.OnPickup();
    }
}
