using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteFrogocite : Frogocite {

    public float jumpTriggerDistance = 2f;

    private Vector2 FindJumpPosition()
    {

        float decrement = 0.5f;

        float i = maxJumpRange;

        while(i >= 1f)
        {

            Vector2 checkPos = Vector2.Lerp(transform.position, transform.position + (transform.position - player.position).normalized * maxJumpRange, i / maxJumpRange);

            checkPos = new Vector2(Mathf.Floor(checkPos.x), Mathf.Floor(checkPos.y));

            if (room.IsTileWalkable(room.groundTilemap, checkPos))
            {
                return checkPos;
            }

            i -= decrement;
        }

        i = maxJumpRange;

        while (i >= 1f)
        {

            Vector2 checkPos = Vector2.Lerp(transform.position, transform.position + (player.position - transform.position).normalized * maxJumpRange, i / maxJumpRange);

            checkPos = new Vector2(Mathf.Floor(checkPos.x), Mathf.Floor(checkPos.y));

            if (room.IsTileWalkable(room.groundTilemap, checkPos))
            {
                return checkPos;
            }

            i -= decrement;
        }

        return Vector2.zero;
    }

    protected override void DoJump()
    {
        if (!isJumping && !isKnockedBack && timeToJump >= jumpCooldown && (playerState == PlayerState.ATTACKING || playerState == PlayerState.CHASING_ENEMY))
        {
            if(Vector2.Distance(transform.position, player.position) < jumpTriggerDistance)
            {
                Vector2 jumpPos = FindJumpPosition();
                if(!jumpPos.Equals(Vector2.zero))
                    Jump(jumpPos + new Vector2(0.5f, 0.5f));

            }
            
        }
    }

    protected override void OnLand()
    {

    }
}
