using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : AnimationMama {

    protected AIPath path;

    protected virtual void Start()
    {
        path = transform.parent.GetComponent<AIPath>();
        animator = GetComponent<Animator>();
      //  vector2Before = path.desiredVelocity;
    }


    protected override void updateVector2()
    {
        //  vector2Before = vector2;
        if (path)
        {
            Vector3 vector3D = path.desiredVelocity;
            vector2 = vector3D;
        }

    }
    protected override void FlipAnimation()
    {
        base.FlipAnimation();
        
    }

    protected virtual void animation360()
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
