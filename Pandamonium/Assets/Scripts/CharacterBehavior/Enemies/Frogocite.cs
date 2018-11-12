using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frogocite : Enemy
{

    public float jumpSpeed;  //allows us to be able to change speed in Unity
    public Vector2 jumpHeight = new Vector2(0, 10);

    public override void Start()
    {
        base.Start();
        jumpSpeed = normalSpeed * 1.2f;
    }

    protected override void Update()
    {
        /*
        if (timeToDash >= dashCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange))
        {
            StartCoroutine(Dash(player));
        }
        */


        if ( CanSee(player, maxDashRange))
            Jump();

        base.Update();
    }

    private void Jump()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(rb.velocity, ForceMode2D.Impulse);

    }
}
