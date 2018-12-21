using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : CharacterAnimation
{
    public PlayerWithJoystick player;

    protected override void updateVector2()
    {
        if (GameManager.joystick)
            vector2 = (GameManager.I.playerInstance as PlayerWithJoystick).facingDirection;
        else base.updateVector2();
    }

    void Update()
    {
        animation360();
    }

    protected override void animation360()
    {
        updateVector2();

        bool isIdle = Mathf.Approximately(vector2.x, Vector2.zero.x) && Mathf.Approximately(vector2.y, Vector2.zero.y);

        if (isIdle)
            animator.SetLayerWeight(1, 0);

        else
        {
            animator.SetLayerWeight(1, 1);
            updateAngleTo360();
            animator.SetFloat("Angle", angle);
        }
    }


}

