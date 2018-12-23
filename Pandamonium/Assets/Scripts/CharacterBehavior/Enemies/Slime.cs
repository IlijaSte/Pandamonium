using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : StandardEnemy {

    public float dashCastTime = 1;

    public float dashCooldown = 6;
    public float dashSpeed = 12;
    public float maxDashRange = 4;

    protected float timeToDash;
    protected bool isCharging = false;

    public override void Start()
    {
        base.Start();
        timeToDash = dashCooldown;
    }

    private float stopDashingAt;

    protected Transform dashingAt = null;

    protected IEnumerator Dash(Vector3 to)
    {
        if (playerState == PlayerState.DASHING || playerState == PlayerState.IMMOBILE || timeToDash < dashCooldown)
            yield break;

        timeToDash = 0;

        StopAttacking();
        stopDashingAt = 0;

        // krecemo da hodamo do cilja samo dok ne nadjemo celu putanju, nakon cega se ubrzavamo

        MoveToPosition(to);

        while (path.pathPending)
            yield return new WaitForEndOfFrame();

        if (playerState != PlayerState.WALKING)
            yield break;

        playerState = PlayerState.DASHING;

        if (path.remainingDistance > maxDashRange)
        {
            stopDashingAt = path.remainingDistance - maxDashRange;
        }

        path.maxSpeed = dashSpeed;


    }

    // za slucaj dash-ovanja na nesto umesto samo u tacku

    protected IEnumerator Dash(Transform at)
    {
        timeToDash = 0;
        if (playerState != PlayerState.IMMOBILE)
        {
            yield return StartCoroutine(Dash(at.position + (transform.position - at.position) * 0.4f));

            // dashingAt cuva za kasnije (u Update kada zavrsi Dash) za damage-ovanje - izmeniti
            dashingAt = at;
        }
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

    public override bool TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        if (isCharging)
        {
            isCharging = false;
            playerState = PlayerState.IDLE;
        }

        return base.TakeDamageWithKnockback(damage, dir, force);

    }

    protected override void Update()
    {
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange - 0.5f))
        {
            StartCoroutine(StartDashing());
        }

        timeToDash += Time.deltaTime;

        base.Update();

        if(playerState == PlayerState.DASHING)
        {

            if ((stopDashingAt == 0 && path.reachedEndOfPath) || (Mathf.Approximately(path.velocity.x, 0) && Mathf.Approximately(path.velocity.y, 0)))      // ako je stigao do destinacije
            {

                path.maxSpeed = normalSpeed;

                if (dashingAt)
                {
                    if (weapons[equippedWeaponIndex].IsInRange(dashingAt))
                        dashingAt.GetComponent<AttackingCharacter>().TakeDamage(weapons[equippedWeaponIndex].damage);
                    Attack(dashingAt);
                    dashingAt = null;
                }
                else
                {
                    if (type == CharacterType.ENEMY && weapons[equippedWeaponIndex].IsInRange(GameManager.I.playerInstance.transform))
                    {
                        GameManager.I.playerInstance.TakeDamage(weapons[equippedWeaponIndex].damage);
                    }
                    playerState = PlayerState.IDLE;
                }

            }
            else if (stopDashingAt > 0 && path.remainingDistance < stopDashingAt)
            {


                path.maxSpeed = normalSpeed;

                if (dashingAt)
                {
                    Attack(dashingAt);
                    dashingAt = null;

                }
                else
                {
                    if (type == CharacterType.ENEMY && weapons[equippedWeaponIndex].IsInRange(GameManager.I.playerInstance.transform))
                    {
                        GameManager.I.playerInstance.TakeDamage(weapons[equippedWeaponIndex].damage);
                    }
                    playerState = PlayerState.WALKING;
                }
            }
        }
    }

    protected override void Die()
    {

        DropItem();

        base.Die();
    }
}
