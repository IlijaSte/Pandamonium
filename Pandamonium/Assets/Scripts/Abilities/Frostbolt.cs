using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostbolt : Ability {

    public float projectileSpeed;

    public GameObject frostboltPrefab;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        // kreiranje projektila na mestu nosioca
        GameObject projectile = Instantiate(frostboltPrefab);
        projectile.transform.position = fromPosition;

        // ispaljivanje projektila

        Transform facingEnemy = GetFacingEnemy();

        if (facingEnemy)
            projectile.GetComponent<FrostboltProjectile>().Shoot(am.parent.transform, facingEnemy, damage, range, projectileSpeed, knockback, knockbackForce);
        else
            projectile.GetComponent<FrostboltProjectile>().Shoot(am.parent.transform, direction, damage, range, projectileSpeed, knockback, knockbackForce);
    }
}
