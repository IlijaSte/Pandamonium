using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability {

    //public float dashSpeed = 5;

    public float maxDashRange = 4;

    private Rigidbody2D rb;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        rb = am.parent.GetComponent<Rigidbody2D>();

        DoDash();

    }

    void DoDash()
    {
        rb.AddForce(am.parent.GetFacingDirection() * maxDashRange * rb.drag, ForceMode2D.Impulse);
    }

}
