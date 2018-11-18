using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : CharacterAnimation
{


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

}

