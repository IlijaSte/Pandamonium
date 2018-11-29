using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrbProjectile : Projectile {

    public HazardousArea area;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        rb.MoveRotation(rb.rotation - Time.deltaTime * 360);
        base.Update();

    }

    protected override void OnHitEnemy(AttackingCharacter enemyHit)
    {
        base.OnHitEnemy(enemyHit);

        if (ability.knockback)
        {
            area.DealDamageWithKnockback(damage, ability.knockbackForce);
        }
        else
        {
            area.DealDamage(damage);
        }
    }

}
