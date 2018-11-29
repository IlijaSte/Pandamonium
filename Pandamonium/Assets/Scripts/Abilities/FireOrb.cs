using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrb : Ability {

    public float projectileSpeed;

    public GameObject fireOrbPrefab;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        // kreiranje projektila na mestu nosioca
        GameObject projectile = Instantiate(fireOrbPrefab);
        projectile.transform.position = fromPosition;

        // ispaljivanje projektila

        Transform facingEnemy = GetFacingEnemy();

        if (facingEnemy)
            projectile.GetComponent<FireOrbProjectile>().Shoot(this, facingEnemy, projectileSpeed);
        else
            projectile.GetComponent<FireOrbProjectile>().Shoot(this, direction, projectileSpeed);
    }
}
