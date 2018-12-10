using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// primer jednog ranged oruzja
public class FireStaff : RangedWeapon
{

    public GameObject firePrefab;   // prefab projektila (vatre)

    public override bool Attack(Transform target)
    {
        if (timeToAttack <= 0)
        {
            // kreiranje projektila na mestu nosioca
            GameObject projectile = Instantiate(firePrefab);
            projectile.transform.position = transform.position;

            // ispaljivanje projektila
            projectile.GetComponent<FireProjectile>().Shoot(parent.transform, target, damage, range, projectileSpeed, knockback, knockbackForce);

            base.Attack(target);
            return true;
        }

        return false;
    }

    public override bool AttackInDirection(Vector2 direction)
    {
        
        if (timeToAttack <= 0)
        {

            // kreiranje projektila na mestu nosioca
            GameObject projectile = Instantiate(firePrefab);
            projectile.transform.position = transform.position;

            // ispaljivanje projektila
            //projectile.GetComponent<FireProjectile>().Shoot(transform, direction, projectileSpeed);

            base.AttackInDirection(direction);
            return true;
        }

        return false;

    }
}
