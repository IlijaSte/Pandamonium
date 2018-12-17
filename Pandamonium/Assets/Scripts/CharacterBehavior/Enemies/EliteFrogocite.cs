using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteFrogocite : Frogocite {

    public float jumpTriggerDistance = 3f;

    public bool fleeing = false;

    private float lastFlee;
    private float fleeCheckTime = 1;

    public override void Start()
    {
        base.Start();

        lastFlee = Time.time;
    }

    private Vector2 FindJumpPosition()
    {

        float decrement = 0.5f;
        float i;

        float maxDistance = 0;
        Vector2 maxPos = Vector2.zero;

        for (int j = 0; j < 4; j++)
        {
            i = maxJumpRange;

            while (i >= 1f && i > maxDistance)
            {
                Vector2 checkPos = Vector2.zero;
                switch (j)
                {
                    // od igraca
                    case 0:
                        checkPos = Vector2.Lerp(transform.position, transform.position + (transform.position - player.position).normalized * maxJumpRange, i / maxJumpRange);
                        break;
                    // levo od igraca
                    case 1:
                        checkPos = Vector2.Lerp(transform.position, (Vector2)transform.position + -Vector2.Perpendicular(((Vector2)(transform.position - player.position)).normalized) * maxJumpRange, i / maxJumpRange);
                        break;
                    // desno od igraca
                    case 2:
                        checkPos = Vector2.Lerp(transform.position, (Vector2)transform.position + Vector2.Perpendicular(((Vector2)(transform.position - player.position)).normalized) * maxJumpRange, i / maxJumpRange);
                        break;
                    // preko igraca
                    default:
                        checkPos = Vector2.Lerp(transform.position, transform.position + (player.position - transform.position).normalized * maxJumpRange, i / maxJumpRange);
                        break;
                }

                if (j != 3 || maxDistance < 2)
                {

                    checkPos = new Vector2(Mathf.Floor(checkPos.x), Mathf.Floor(checkPos.y));

                    if (room.IsTileWalkable(room.groundTilemap, checkPos))
                    {
                        maxDistance = i;
                        maxPos = checkPos;
                        break;
                    }
                }

                i -= decrement;
            }
        }

        return maxPos;
    }

    protected override void DoJump()
    {

    }

    public override void OnWeaponAttack()
    {
        base.OnWeaponAttack();

        float distance = Vector2.Distance(transform.position, player.position);

        if (!isJumping && !isKnockedBack && distance < jumpTriggerDistance)
        {
            fleeing = true;

            Vector2 gotoPos = FindJumpPosition();

            if (timeToJump >= jumpCooldown)
            {

                if (!gotoPos.Equals(Vector2.zero))
                    Jump(gotoPos + new Vector2(0.5f, 0.5f));
            }
            else
            {
                MoveToPosition(gotoPos + new Vector2(0.5f, 0.5f));
            }
        }
        else if(!isJumping && !isKnockedBack)
        {
            fleeing = false;
        }


    }

    protected bool FrogocidsInRoom()
    {
        foreach(GameObject enemy in room.enemies)
        {
            if(enemy && enemy.GetComponent<Frogocite>() != null && enemy.GetComponent<EliteFrogocite>() == null)
            {
                return true;
            }
        }

        return false;
    }

    protected override void Update()
    {
        if (isDead)
            return;

        base.Update();

        if (Time.time - lastFlee >= fleeCheckTime && FrogocidsInRoom())
        {
            
            if (!isJumping && !isKnockedBack && (playerState == PlayerState.ATTACKING || playerState == PlayerState.CHASING_ENEMY || playerState == PlayerState.WALKING))
            {
                float dist = Vector2.Distance(transform.position, player.position);

                if (dist < jumpTriggerDistance)
                {
                    Vector2 fleePos = FindJumpPosition() + new Vector2(0.5f, 0.5f);
                    if (timeToJump < jumpCooldown)
                    {
                        MoveToPosition(fleePos);
                    }
                    else
                    {
                        Jump(fleePos);
                    }

                    fleeing = true;
                    lastFlee = Time.time;
                }
                else
                {
                    fleeing = false;
                }
            }
        }

    }

    protected override void OnLand()
    {

    }
}
