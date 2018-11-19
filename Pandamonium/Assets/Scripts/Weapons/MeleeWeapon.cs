using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    override public void Attack(Transform target)
    {
        if (timeToAttack <= 0)
        {
            if (knockback)
            {
                target.GetComponent<AttackingCharacter>().TakeDamageWithKnockback(damage, (target.position - transform.position).normalized, knockbackForce);
            }
            else
            {
                target.GetComponent<AttackingCharacter>().TakeDamage(damage);
            }

            base.Attack(target);
        }
    }
}
