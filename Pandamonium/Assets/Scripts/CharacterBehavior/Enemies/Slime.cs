using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy {

    protected override void Update()
    {
        if (!dashed && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange))
        {
            dashed = true;
            StartCoroutine(Dash(player));
        }
        base.Update();
    }
}
