using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frogocite : Enemy
{

    public float jumpSpeed = 5;
    private Vector2 jumpTarget;
    public float maxJumpRange = 2;

    private Rigidbody2D rb;
    private new CircleCollider2D collider;
    public GameObject indicatorPrefab;
    private Transform indicator;
    private Animator animator;

    public bool isJumping = false;
    private float timeToJump;
    public float jumpCooldown = 6;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        timeToJump = jumpCooldown;
    }

    protected override void Update()
    {
        if (timeToJump < jumpCooldown)
        {
            timeToJump += Time.deltaTime;
        }

        if (!isJumping && timeToJump >= jumpCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxJumpRange))
        {
            Jump(player.position);
        }
      
        base.Update();
    }

    private Vector2 GetInitVelocity()
    {
        float g = -Physics2D.gravity.y;
        float x = jumpTarget.x - transform.position.x;
        float y = jumpTarget.y - transform.position.y;

        float b;
        float discriminant;
        jumpSpeed -= 5;

        do
        {
            jumpSpeed += 5;

            b = jumpSpeed * jumpSpeed - y * g;
            discriminant = b * b - g * g * (x * x + y * y);

        } while (discriminant < 0);

        float discRoot = Mathf.Sqrt(discriminant);

        // Impact time for the most direct shot that hits.
        float T_min = Mathf.Sqrt((b - discRoot) * 2 / (g * g));

        // Impact time for the highest shot that hits.
        float T_max = Mathf.Sqrt((b + discRoot) * 2 / (g * g));

        float T = (T_max + T_min) / 2;

        float vx = x / T;
        float vy = y / T + T * g / 2;

        Vector2 velocity = new Vector2(vx, vy);

        return velocity;
    }

    public void Jump(Vector2 target)
    {
        playerState = PlayerState.WALKING;
        isJumping = true;
        this.jumpTarget = new Vector2(target.x, target.y);

        //collider = GetComponent<CircleCollider2D>();
        //collider.enabled = false;

        path.enabled = false;
        //path.isStopped = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<BoxCollider2D>().enabled = false;
        rb.AddForce(GetInitVelocity(), ForceMode2D.Impulse);

        indicator = Instantiate(indicatorPrefab, jumpTarget, Quaternion.identity).transform;

        timeToJump = 0;
    }

    public void FixedUpdate()
    {
        if (isJumping && rb.velocity.y < 0 && transform.position.y <= jumpTarget.y)
        {
            playerState = PlayerState.CHASING_ENEMY;

            isJumping = false;

            Destroy(indicator.gameObject);

            path.enabled = true;
            //path.isStopped = false;

            GetComponent<BoxCollider2D>().enabled = true;
 
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
