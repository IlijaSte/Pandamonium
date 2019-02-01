using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// primer jednog ranged oruzja
public class FireStaff : RangedWeapon
{

    public GameObject firePrefab;   // prefab projektila (vatre)

    public override bool Attack(Transform target)
    {
        if (timeToAttack <= 0 && (am == null || am.globalCDProgress >= am.globalCooldown))
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

    public override bool AttackInDirection(Vector2 direction, bool regenMana = false)
    {
        
        if (timeToAttack <= 0 && (am == null || am.globalCDProgress >= am.globalCooldown))
        {

            if (animator)
            {
                animator.WeaponStrike();
            }

            // kreiranje projektila na mestu nosioca
            GameObject projectile = Instantiate(firePrefab);
            projectile.transform.position = transform.position;

            Transform target = autolock.GetClosest();

            PlayerWithJoystick player = parent.GetComponent<PlayerWithJoystick>();
            int bonusDamage = (player ? player.GetDamage() : 0);

            // ispaljivanje projektila

            if (target)
            {
                projectile.GetComponent<FireProjectile>().Shoot(parent.transform, target, damage + bonusDamage, range, projectileSpeed, knockback, knockbackForce, regenMana);
            }
            else
            {
                projectile.GetComponent<FireProjectile>().Shoot(parent.transform, direction, damage + bonusDamage, range, projectileSpeed, knockback, knockbackForce, regenMana);
            }
            base.AttackInDirection(direction);

            return true;
        }

        return false;

    }
}
