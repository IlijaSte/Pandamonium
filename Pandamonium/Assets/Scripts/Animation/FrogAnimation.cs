using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimation : CharacterAnimation
{
  

    void Update()
    {
        FlipAnimation();
        bool isIdle = Mathf.Approximately(vector2.x, Vector2.zero.x) && Mathf.Approximately(vector2.y, Vector2.zero.y);
        if (isIdle)
            animator.SetBool("Walking", false);
        else
            animator.SetBool("Walking", true);
    }

}

