using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWithJoystick : AttackingCharacter {

    public JoystickController controller;

    [HideInInspector]
    public Vector2 facingDirection;

    public float lockAngle = 60;

    public override void Awake()
    {
        if (!GameManager.joystick)
        {
            controller.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else{
            GameManager.I.playerInstance = this;
        }
    }

    public override void Start()
    {
        base.Start();
        facingDirection = Vector2.zero;
    }

    protected IEnumerator Dash()
    {
        if (playerState != PlayerState.DASHING && !facingDirection.Equals(Vector2.zero))
        {
            float startTime = Time.time;
            Vector2 startPos = transform.position;

            playerState = PlayerState.DASHING;

            while (Time.time - startTime < 0.3f && Vector2.Distance(startPos, transform.position) < maxDashRange)
            {
                rb.AddForce(facingDirection * dashSpeed * 20, ForceMode2D.Force);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, dashSpeed);

                yield return new WaitForEndOfFrame();
            }

            playerState = PlayerState.IDLE;
        }
    }

    protected override void Update()
    {
        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack <= 0)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
        }
    }

    protected void FixedUpdate()
    {

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {

            if (playerState != PlayerState.DASHING)
            {

                Vector2 wasdDirection = Vector2.zero;

                if (Input.GetKey(KeyCode.A))
                {
                    wasdDirection += Vector2.left;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    wasdDirection += Vector2.right;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    wasdDirection += Vector2.up;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    wasdDirection += Vector2.down;
                }



                if (!wasdDirection.Equals(Vector2.zero))
                {
                    wasdDirection = wasdDirection.normalized;

                    if (rb.velocity.magnitude < normalSpeed)
                    {
                        rb.AddForce(wasdDirection * normalSpeed * 20, ForceMode2D.Force);
                        facingDirection = wasdDirection;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Attack();
            }

            if (Input.GetMouseButtonDown(1))
            {
                // DASH
                StartCoroutine(Dash());
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                ChangeWeapon();
                UIManager.I.weaponChange.changeText();
            }
        }
        else
        {


            facingDirection = rb.velocity.normalized;

            if (!Mathf.Approximately(controller.InputDirection.x, 0) || !Mathf.Approximately(controller.InputDirection.y, 0))
            {
                if (rb.velocity.magnitude < normalSpeed)
                {
                    facingDirection = controller.InputDirection.normalized;
                    rb.AddForce(controller.InputDirection * normalSpeed * 20, ForceMode2D.Force);
                }

            }
        }
    }

    protected Transform GetFacingEnemy()
    {

        List<Transform> visibleTargets = new List<Transform>();

        float lockRadius = weapons[equippedWeaponIndex].range;

        float minDistance = Mathf.Infinity;
        Transform closestTarget = null;

        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, lockRadius, ignoreMask);

        for(int i = 0; i < targetsInRadius.Length; i++)
        {
            Vector2 dirToTarget = (targetsInRadius[i].transform.position - transform.position).normalized;

            if(Vector2.Angle(facingDirection, dirToTarget) < lockAngle / 2)
            {
                float distance = Vector2.Distance(transform.position, targetsInRadius[i].transform.position);

                if (distance < minDistance)
                {
                    if (CanSee(targetsInRadius[i].transform, distance))
                    {
                        minDistance = distance;
                        closestTarget = targetsInRadius[i].transform;
                    }
                }
            }

            
        }

        return closestTarget;
    }

    public void Attack()
    {

        Transform facingEnemy;

        if (facingEnemy = GetFacingEnemy())
        {
            weapons[equippedWeaponIndex].Attack(facingEnemy);
        }
        else
        {
            weapons[equippedWeaponIndex].AttackInDirection(facingDirection);
        }

    }

    public override void TakeDamage(float damage)
    {
        if (!isDead)
        {
            base.TakeDamage(damage);

            healthBar.fillAmount = health / maxHealth;
        }
    }

    public override void Die()
    {
        MenuManager.I.ShowMenu(MenuManager.I.deathMenu);
        isDead = true;
        //base.Die();
    }
}
