﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frogocite : Enemy
{

    public float jumpSpeed = 5;
    private Vector2 jumpTarget;
    public float maxJumpRange = 2;

    private BoxCollider2D boxCollider2D;
    //private Rigidbody2D rb;
    private new CircleCollider2D collider;
    public GameObject indicatorPrefab;
    private Transform indicator;
    private Animator animator;

    public bool isJumping = false;
    private float timeToJump;
    public float jumpCooldown = 6;

    private Transform shadow;
    private Vector2 oldShadowPos;
    private Vector2 oldShadowRelativePos;
    private float distance;

    public override void Start()
    {
        base.Start();
        //rb = GetComponent<Rigidbody2D>();
        timeToJump = jumpCooldown;
        boxCollider2D = GetComponent<BoxCollider2D>();

        shadow = sprite.transform.GetChild(0);
    }

    protected override void Update()
    {
        if (timeToJump < jumpCooldown)
        {
            timeToJump += Time.deltaTime;
        }

        if (!isJumping && !isKnockedBack && timeToJump >= jumpCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxJumpRange))
        {
            Jump(new Vector2(player.position.x, player.position.y));
        }
      
        base.Update();
    }

    private Vector2 GetInitVelocity()
    {
        float g = -Physics2D.gravity.y;
        float x = jumpTarget.x - transform.position.x;
        float y = jumpTarget.y - transform.position.y;

        distance = Vector2.Distance(transform.position, jumpTarget);

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
        timeToJump = 0;
        playerState = PlayerState.IMMOBILE;
        isJumping = true;
        this.jumpTarget = target;// new Vector2(target.x, target.y);

        //collider = GetComponent<CircleCollider2D>();
        //collider.enabled = false;

        path.enabled = false;
        //path.isStopped = true;


        //boxCollider2D.enabled = false;
        boxCollider2D.isTrigger = true;


        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero; 
        rb.AddForce(GetInitVelocity() * rb.mass, ForceMode2D.Impulse);
        

        indicator = Instantiate(indicatorPrefab, jumpTarget, Quaternion.identity).transform;

        oldShadowRelativePos = shadow.localPosition;
        shadow.SetParent(null);
        oldShadowPos = shadow.position;

        attackable = false;
       // timeToJump = 0;
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

            //GetComponent<BoxCollider2D>().enabled = true;
            boxCollider2D.isTrigger = false;

            rb.bodyType = RigidbodyType2D.Kinematic;

            if (weapons[equippedWeaponIndex].IsInRange(player))
                player.GetComponent<AttackingCharacter>().TakeDamage(weapons[equippedWeaponIndex].damage);
            Attack(player);

            shadow.SetParent(sprite.transform);
            shadow.localPosition = oldShadowRelativePos;

            attackable = true;
        }

        if (isJumping)
        {
            shadow.position = new Vector2(transform.position.x, Mathf.Lerp(oldShadowPos.y, jumpTarget.y, 1 - Vector2.Distance(jumpTarget, transform.position) / distance));
        }
    }

    public override void TakeDamage(float damage)
    {
        if(!isJumping)
            base.TakeDamage(damage);
    }

    public override void TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        if(!isJumping)
            base.TakeDamageWithKnockback(damage, dir, force);
    }
}
