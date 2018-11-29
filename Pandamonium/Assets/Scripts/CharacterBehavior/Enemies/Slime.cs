using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy {

    public float dashCastTime = 1;

    public override void Start()
    {
        base.Start();
    }

    protected IEnumerator StartDashing()
    {
        StopAttacking();
        StopMoving();
        playerState = PlayerState.IMMOBILE;

        yield return new WaitForSeconds(dashCastTime);

        playerState = PlayerState.IDLE;
        StartCoroutine(Dash(player));       // promeniti zbog cast vremena
    }

    protected override void Update()
    {
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange - 0.5f))
        {
            StartCoroutine(StartDashing());
        }

        base.Update();
    }
}
