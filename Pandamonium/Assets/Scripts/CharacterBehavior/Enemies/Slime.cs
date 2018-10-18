using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy {

    protected override void Update()
    {
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange))
        {
            StartCoroutine(Dash(player));
        }

        base.Update();
    }
}
