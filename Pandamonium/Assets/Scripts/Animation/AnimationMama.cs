using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationMama : MonoBehaviour {

    protected Animator animator;
    protected float angle;
    protected Vector3 vector3;
    protected Vector2 vector2;
    protected Vector2 vector2Before;

    protected abstract void updateVector2();

    protected virtual void FlipAnimation()
    {
        updateVector2();

        angle = Vector2.Angle(vector2, new Vector2(1, 0));

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //if ((angle > 0 && angle < 90))
 
        if(vector2.x < 0)
            sr.flipX = false;
        else if (vector2.x > 0) sr.flipX = true;
    }
    protected virtual void updateAngleTo360()
    {

        if (vector2.y < 0)
            angle = 180 + 180 - Vector2.Angle(vector2, new Vector2(1, 0));
        else
            angle = Vector2.Angle(vector2, new Vector2(1, 0));

    }
}
