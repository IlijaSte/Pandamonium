using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Ability {

    public float projectileSpeed;

    public GameObject fireballPrefab;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        // kreiranje projektila na mestu nosioca
        GameObject projectile = Instantiate(fireballPrefab);
        projectile.transform.position = fromPosition;

        // ispaljivanje projektila

        Transform facingEnemy = GetFacingEnemy();

        if(facingEnemy)
            projectile.GetComponent<FireProjectile>().Shoot(am.parent.transform, facingEnemy, damage, range, projectileSpeed, knockback, knockbackForce);
        else
            projectile.GetComponent<FireProjectile>().Shoot(am.parent.transform, direction, damage, range, projectileSpeed, knockback, knockbackForce);
    }
}
