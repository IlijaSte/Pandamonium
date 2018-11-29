using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability {

    public float dashSpeed = 5;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        StartCoroutine(DoDash(direction));
    }

    protected IEnumerator DoDash(Vector2 direction)
    {

        Rigidbody2D rb = am.parent.GetComponent<Rigidbody2D>();

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
    }

}
