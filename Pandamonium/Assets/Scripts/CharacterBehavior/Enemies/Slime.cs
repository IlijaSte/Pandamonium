using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy {

    public override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange - 0.5f))
        {
            StartCoroutine(Dash(player));
        }

        base.Update();
    }
}
