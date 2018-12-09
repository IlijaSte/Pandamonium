using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWithJoystick : AttackingCharacter {

    public float maxEnergy = 100;

    [HideInInspector]
    public float energy = 0;

    public float lockAngle = 60;

    public Image energyBar;

    public AbilityManager abilityManager;
    public JoystickController controller;

    [HideInInspector]
    public Vector2 facingDirection = Vector2.down;

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

        timeToDash = dashCooldown;

        if(abilityManager == null)
        {
            abilityManager = GetComponentInChildren<AbilityManager>();
        }
    }

    public void PickupBlueprint(Blueprint bp)
    {
        abilityManager.AddAbility(bp);
    }

    protected override void Update()
    {
        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack <= 0)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
        }

        energyBar.fillAmount = energy / maxEnergy;
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

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    normalSpeed = 30;
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    normalSpeed = 6;
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
                //Attack();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                // DASH
                abilityManager.UseAbility(0);
            }

            if(Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Alpha1))
            {
                abilityManager.StopUsingAbility(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                abilityManager.UseAbility(1);
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                abilityManager.StopUsingAbility(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                abilityManager.UseAbility(2);
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                abilityManager.StopUsingAbility(2);
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                ChangeWeapon();
            }
        }
        else
        {

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

    public override Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    protected void IncreaseEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
    }

    public void DecreaseEnergy(float amount)
    {
        energy = Mathf.Clamp(energy - amount, 0, maxEnergy);
    }

    public override void Attack()
    {

        if (weapons[equippedWeaponIndex] is MeleeWeapon)
        {
            int numHit = ((MeleeWeapon)weapons[equippedWeaponIndex]).AttackCleave();
            IncreaseEnergy(numHit * weapons[equippedWeaponIndex].damage);
        }

    }

    public void UseAbility(int abilityIndex)
    {
        abilityManager.UseAbility(abilityIndex);

    }

    public void StopUsingAbility(int abilityIndex)
    {
        abilityManager.StopUsingAbility(abilityIndex);
    }

    public override void TakeDamage(float damage)
    {
        if (!isDead)
        {
            base.TakeDamage(damage);

            healthBar.FillAmount(health / maxHealth);
        }
    }

    public override void TakePoisonDamage(float damage)
    {
        
        if (!isDead)
        {
            base.TakePoisonDamage(damage);

            healthBar.FillAmount(health / maxHealth);
        }
    }


    public override void Die()
    {
        if (!isDead)
        {
            MenuManager.I.ShowMenu(MenuManager.I.deathMenu);
            isDead = true;
            //base.Die();
        }
    }
}
