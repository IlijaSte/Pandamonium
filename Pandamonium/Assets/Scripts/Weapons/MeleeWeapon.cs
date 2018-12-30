using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float cleaveAngle = 180;
    public WeaponAnimation animator;

    override public bool Attack(Transform target)
    {
        if (timeToAttack <= 0 && (am == null || am.globalCDProgress >= am.globalCooldown))
        {
            Damage(target);

            base.Attack(target);
            return true;
        }

        return false;
    }

    protected bool Damage(Transform target, float damageAddition = 0)
    {
        bool takenDamage = false;

        if (knockback && target.GetComponent<AttackingCharacter>() != null)
        {  
            takenDamage = target.GetComponent<AttackingCharacter>().TakeDamageWithKnockback(damage + damageAddition, (target.position - transform.position).normalized, knockbackForce);
        }
        else
        {
            if (target.GetComponent<AttackableObject>() != null)
            {
                takenDamage = target.GetComponent<AttackableObject>().TakeDamage(damage + damageAddition);
            }
            else
            {
                takenDamage = target.GetComponent<AttackingCharacter>().TakeDamage(damage + damageAddition);
            }
        }

        return takenDamage;
    }

    public int AttackCleave(float damageAddition)
    {

        if (timeToAttack > 0 || (am != null && am.globalCDProgress < am.globalCooldown))
            return 0;

        if(animator)
            animator.WeaponStrike();

        if(am != null)
        {
            StartCoroutine(am.GlobalCooldown());
        }

        int damagedTargets = 0;

        Transform[] inRange = new Transform[autolock.enemiesInRange.Count];

        autolock.enemiesInRange.CopyTo(inRange);

        foreach(Transform target in inRange)
        {
            if (target != null && IsInRange(target) && Damage(target, damageAddition))
            {
                damagedTargets++;
            }
        }

        timeToAttack = 1;

        return damagedTargets;
    }
}
