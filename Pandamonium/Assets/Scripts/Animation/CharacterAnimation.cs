using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : AnimationMama {

    protected AIPath path;

    void Start()
    {
        path = transform.parent.GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        vector2Before = path.desiredVelocity;
    }


    protected override void updateVector2()
    {
        vector2Before = vector2;
        Vector3 vector3D = path.desiredVelocity;
        vector2 = vector3D;
    }

    protected void animation360()
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


    protected void updateAngleTo360()
    {

        if (vector2.y < 0)
            angle = 180 + 180 - Vector2.Angle(vector2, new Vector2(1, 0));
        else
            angle = Vector2.Angle(vector2, new Vector2(1, 0));
  
    }
}
