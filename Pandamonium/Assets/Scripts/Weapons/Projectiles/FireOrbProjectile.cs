using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrbProjectile : Projectile {

    public EnemyHazardousArea area;
    public GameObject aoeIndicator;

    protected Rigidbody2D rb;

    protected float secondaryDamage;

    FireOrb ability;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        rb.MoveRotation(rb.rotation - Time.deltaTime * 360);
        base.Update();

    }

    public virtual void Shoot(FireOrb ability, Vector2 direction)
    {
        base.Shoot(ability.am.parent.transform, direction, ability.damage, ability.range, ability.projectileSpeed, ability.knockback, ability.knockbackForce);
        this.ability = ability;
        secondaryDamage = ability.aoeDamage;
    }

    public virtual void Shoot(FireOrb ability, Transform target)
    {
        base.Shoot(ability.am.parent.transform, target, ability.damage, ability.range, ability.projectileSpeed, ability.knockback, ability.knockbackForce);
        this.ability = ability;
        secondaryDamage = ability.aoeDamage;
    }

    protected override void OnEndOfRange()
    {
        base.OnEndOfRange();

        if (knockback)
        {
            area.DealDamageWithKnockback(secondaryDamage, kbForce);
        }
        else
        {
            area.DealDamage(secondaryDamage);
        }

        ability.ShowAoeIndicator(transform.position, aoeIndicator);
    }

    protected override void OnHitEnemy(AttackingCharacter enemyHit)
    {
        base.OnHitEnemy(enemyHit);

        if (knockback)
        {
            area.DealDamageWithKnockback(secondaryDamage, kbForce);
        }
        else
        {
            area.DealDamage(secondaryDamage);
        }

        ability.ShowAoeIndicator(transform.position, aoeIndicator);
    }

}
