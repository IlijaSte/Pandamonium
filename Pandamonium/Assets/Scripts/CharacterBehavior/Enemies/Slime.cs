using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy {

    public float dashCastTime = 1;

    protected bool isCharging = false;

    public override void Start()
    {
        base.Start();
    }

    protected IEnumerator StartDashing()
    {

        Vector2 targetPos = player.position;

        StopAttacking();
        StopMoving();
        playerState = PlayerState.IMMOBILE;
        isCharging = true;

        yield return new WaitForSeconds(dashCastTime);

        if (isCharging)
        {
            playerState = PlayerState.IDLE;
            StartCoroutine(Dash(targetPos));       // promeniti zbog cast vremena
            isCharging = false;
        }
    }

    public override void TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        if (isCharging)
        {
            isCharging = false;
            playerState = PlayerState.IDLE;
        }

        base.TakeDamageWithKnockback(damage, dir, force);

    }

    protected override void Update()
    {
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange - 0.5f))
        {
            StartCoroutine(StartDashing());
        }

        base.Update();
    }

    public override void Die()
    {

        DropItem();

        base.Die();
    }
}
