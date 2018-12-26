using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyFrogo : Enemy
{
    public float aoeRange = 1;
    public float aoeDamage = 50;
    public float secondaryAoeRange = 3;
    public float secondaryAoeDamage = 25;

    public float primaryStunDuration = 1.5f;
    public float secondaryStunDuration = 0.5f;

    public float selfStunDuration = 4;
    public int selfStunJumps = 3;
    public int selfStunsToEnrage = 3;

    private int numOfSelfStuns = 0;

    private int numOfJumps = 0;

    private bool isStunned = false;

    public float jumpSpeed = 5;
    private Vector2 jumpTarget;
    public float maxJumpRange = 2;

    private BoxCollider2D boxCollider2D;
    //private Rigidbody2D rb;
    private new CircleCollider2D collider;
    public GameObject primaryIndicatorPrefab;
    public GameObject secondaryIndicatorPrefab;
    private Transform primaryIndicator;
    private Transform secondaryIndicator;
    private Animator animator;

    public bool isJumping = false;
    protected float timeToJump;
    public float jumpCooldown = 6;

    private Transform shadow;
    private Vector2 oldShadowPos;
    private Vector2 oldShadowRelativePos;
    private float T;
    private float shadowT;

    private float oldDrag;

    public override void Start()
    {
        base.Start();
        //rb = GetComponent<Rigidbody2D>();
        timeToJump = jumpCooldown;
        boxCollider2D = GetComponent<BoxCollider2D>();

        shadow = sprite.transform.GetChild(0);

        healthBar = UIManager.I.bossHealthBar;

    }

    protected virtual void DoJump()
    {
        if (!isJumping && !isKnockedBack && timeToJump >= jumpCooldown && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxJumpRange))
        {
            Jump(new Vector2(player.position.x, player.position.y));
        }
    }

    protected override void Update()
    {
        if (isDead)
            return;

        if (timeToJump < jumpCooldown)
        {
            timeToJump += Time.deltaTime;
        }

        DoJump();

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

        T = (T_max + T_min) / 2;

        float vx = x / T;
        float vy = y / T + T * g / 2;

        Vector2 velocity = new Vector2(vx, vy);

        return velocity;
    }

    public void Jump(Vector2 target)
    {

        this.jumpTarget = target;

        Vector2 initVelocity = GetInitVelocity();
        if (initVelocity.Equals(Vector2.zero))
            return;

        timeToJump = 0;
        StopAttacking();
        playerState = PlayerState.IMMOBILE;
        isJumping = true;

        path.enabled = false;

        boxCollider2D.isTrigger = true;

        oldDrag = rb.drag;
        rb.drag = 0;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;

        rb.AddForce(initVelocity * rb.mass, ForceMode2D.Impulse);

        primaryIndicator = Instantiate(primaryIndicatorPrefab, jumpTarget, Quaternion.identity).transform;
        secondaryIndicator = Instantiate(secondaryIndicatorPrefab, jumpTarget, Quaternion.identity).transform;

        primaryIndicator.localScale = new Vector2(aoeRange, aoeRange);
        secondaryIndicator.localScale = new Vector2(secondaryAoeRange, secondaryAoeRange);

        oldShadowRelativePos = shadow.localPosition;
        shadow.SetParent(null);
        oldShadowPos = shadow.position;

        attackable = false;

        shadowT = 0;
        // timeToJump = 0;
    }

    public virtual void FixedUpdate()
    {

        if (isDead)
            return;

        if (isJumping && rb.velocity.y < 0 && transform.position.y <= jumpTarget.y)
        {
            playerState = PlayerState.CHASING_ENEMY;

            isJumping = false;

            Destroy(primaryIndicator.gameObject);
            Destroy(secondaryIndicator.gameObject);

            path.enabled = true;

            boxCollider2D.isTrigger = false;

            rb.drag = oldDrag;

            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            OnLand();

            Attack(player);

            shadow.SetParent(sprite.transform);
            shadow.localPosition = oldShadowRelativePos;

            attackable = true;
        }

        if (isJumping)
        {
            shadowT += Time.deltaTime;
            shadow.position = new Vector2(transform.position.x, Mathf.Lerp(oldShadowPos.y, jumpTarget.y, shadowT / T));
        }
    }

    public override bool TakeDamage(float damage)
    {
        if (!isJumping)
        {
            return base.TakeDamage(damage);
        }
        else
        {
            return false;
        }
    }

    public override bool TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        if (!isJumping)
            //return base.TakeDamageWithKnockback(damage, dir, force);
            return base.TakeDamage(damage);
        else
        {
            return false;
        }
    }

    protected void Enrage()
    {
        weapons[equippedWeaponIndex].damage *= 2;
        aoeDamage *= 2;
        secondaryAoeDamage *= 2;
    }

    protected IEnumerator SelfStun()
    {
        numOfSelfStuns++;

        isStunned = true;

        playerState = PlayerState.IMMOBILE;

        yield return new WaitForSeconds(selfStunDuration);

        playerState = PlayerState.IDLE;

        isStunned = false;

        if(numOfSelfStuns == selfStunsToEnrage)
        {
            Enrage();
        }

        timeToJump = 0;
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);

        UIManager.I.bossHealthBar.BuildHealtBar(maxHealth, false);
    }

    public override void StopAttacking()
    {
        target = null;
        playerState = PlayerState.IDLE;

        if (equippedWeaponIndex < weapons.Length)
            weapons[equippedWeaponIndex].Pause();
    }

    protected override void StartAttacking(Transform target)
    {
        weapons[equippedWeaponIndex].ContinueAttacking(target);                  // krece da napada oruzjem
        playerState = PlayerState.ATTACKING;

        CM.StopMoving();
    }

    protected void OnLand()
    {
        //Transform player = GameManager.I.playerInstance.transform;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= aoeRange)
        {
            player.GetComponent<AttackingCharacter>().TakeDamage(aoeDamage);
            player.GetComponent<PlayerWithJoystick>().Stun(primaryStunDuration);
        }
        else if(distance <= secondaryAoeRange)
        {
            player.GetComponent<AttackingCharacter>().TakeDamage(secondaryAoeDamage);
            player.GetComponent<PlayerWithJoystick>().Stun(secondaryStunDuration);
        }

        if (++numOfJumps == selfStunJumps)
        {
            numOfJumps = 0;
            StartCoroutine(SelfStun());
        }

        GetComponent<CinemachineImpulseSource>().GenerateImpulse((transform.position - player.position).normalized);
    }

    protected override void Die()
    {
        base.Die();
    }
}
