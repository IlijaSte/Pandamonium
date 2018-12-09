using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrbProjectile : Projectile {

    public EnemyHazardousArea area;
    public GameObject aoeIndicator;

    protected Rigidbody2D rb;

    protected float secondaryDamage;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        rb.MoveRotation(rb.rotation - Time.deltaTime * 360);
        base.Update();

    }

    public override void Shoot(Ability ability, Vector2 direction, float speed)
    {
        base.Shoot(ability, direction, speed);

        secondaryDamage = (ability as FireOrb).aoeDamage;
    }

    public override void Shoot(Ability ability, Transform target, float speed)
    {
        base.Shoot(ability, target, speed);

        secondaryDamage = (ability as FireOrb).aoeDamage;
    }

    protected override void OnEndOfRange()
    {
        base.OnEndOfRange();

        if (ability.knockback)
        {
            area.DealDamageWithKnockback(secondaryDamage, ability.knockbackForce);
        }
        else
        {
            area.DealDamage(secondaryDamage);
        }

        (ability as FireOrb).ShowAoeIndicator(transform.position, aoeIndicator);
    }

    protected override void OnHitEnemy(AttackingCharacter enemyHit)
    {
        base.OnHitEnemy(enemyHit);

        if (ability.knockback)
        {
            area.DealDamageWithKnockback(secondaryDamage, ability.knockbackForce);
        }
        else
        {
            area.DealDamage(secondaryDamage);
        }

        (ability as FireOrb).ShowAoeIndicator(transform.position, aoeIndicator);
    }

}
