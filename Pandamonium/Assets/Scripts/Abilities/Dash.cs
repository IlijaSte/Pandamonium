using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability {

    //public float dashSpeed = 5;

    public float maxDashRange = 4;

    private bool isJumping = false;

    private Rigidbody2D rb;

    private Vector2 jumpTarget;

    private Vector2 lastPos;
    private float distancePassed = 0f;

    private Collider2D col;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        rb = am.parent.GetComponent<Rigidbody2D>();
        col = am.parent.GetComponent<Collider2D>();

        DoDash();
        //StartCoroutine(DoDash(direction));

    }

    void DoDash()
    {
        isJumping = true;

        lastPos = transform.position;
        distancePassed = 0f;
        //rb.drag = 0;
        rb.AddForce(am.parent.GetFacingDirection() * maxDashRange * rb.drag, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {

    }
    /*protected IEnumerator DoDash(Vector2 direction)
    {

        

        if (!direction.Equals(Vector2.zero))
        {
             
            float startTime = Time.time;
            Vector2 startPos = am.parent.transform.position;

            am.parent.playerState = AttackingCharacter.PlayerState.DASHING;

            while (Time.time - startTime < 0.3f && Vector2.Distance(startPos, am.parent.transform.position) < range)
            {
                rb.AddForce(direction * dashSpeed * 20, ForceMode2D.Force);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, dashSpeed);

                yield return new WaitForEndOfFrame();
            }

            am.parent.playerState = AttackingCharacter.PlayerState.IDLE;

        }
    }*/

}
