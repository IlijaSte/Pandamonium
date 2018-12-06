using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : Collectible {

    public Ability[] abilities;

    [HideInInspector]
    public Ability ability;

    protected override void Start()
    {
        ability = abilities[Random.Range(0, abilities.Length)];
        base.Start();
    }

    public Ability GetAbility()
    {
        return abilities[Random.Range(0, abilities.Length)];
    }

    protected override void OnPickup()
    {

        (GameManager.I.playerInstance as PlayerWithJoystick).PickupBlueprint(this);

        base.OnPickup();
    }
}
