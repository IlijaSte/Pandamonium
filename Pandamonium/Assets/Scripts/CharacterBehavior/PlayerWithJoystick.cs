using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWithJoystick : AttackingCharacter {

    public JoystickController controller;

    [HideInInspector]
    public Vector2 facingDirection;

    public float lockAngle = 60;

    private bool isDead = false;

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
        if(!Mathf.Approximately(controller.InputDirection.x, 0) || !Mathf.Approximately(controller.InputDirection.y, 0))
        {
            if(rb.velocity.magnitude < normalSpeed)
            {
                facingDirection = controller.InputDirection.normalized;
                rb.AddForce(controller.InputDirection * normalSpeed * 20, ForceMode2D.Force);
            }
            // rb.velocity = controller.InputDirection * normalSpeed;

        }
        else
        {
            //rb.velocity = Vector2.zero;
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
        /*
        Vector3 startCast = transform.position;

        RaycastHit2D[] results = new RaycastHit2D[6];

        for (int i = 0; i < Physics2D.CircleCast(startCast, 0.2f, facingDirection, colFilter, results, weapons[equippedWeaponIndex].range); i++) // ako mu je protivnik vidljiv (od zidova/prepreka)
        {

            AttackingCharacter attChar = results[i].transform.GetComponent<AttackingCharacter>();
            if (attChar && attChar.type == type)
                continue;

            if (results[i].transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                return null;
            }

            if (results[i].transform.CompareTag("Enemy"))
            {
                return results[i].transform;
            }
        }

        return null;
        */
    }

    public void Attack()

    {

        // promeniti pod hitno!!!
        /*
        if (weapons[equippedWeaponIndex] is RangedWeapon)
        {
            weapons[equippedWeaponIndex].AttackInDirection(facingDirection);
        }
        else
        {

            Transform facingEnemy;

            if (facingEnemy = GetFacingEnemy())
            {
                weapons[equippedWeaponIndex].Attack(facingEnemy);
            }
        }*/

        Transform facingEnemy;

        if (facingEnemy = GetFacingEnemy())
        {
            weapons[equippedWeaponIndex].Attack(facingEnemy);
        }
        else
        {
            weapons[equippedWeaponIndex].AttackInDirection(facingDirection);
        }

        //weapons[equippedWeaponIndex].Att
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
