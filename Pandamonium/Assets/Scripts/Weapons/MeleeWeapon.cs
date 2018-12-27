using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float cleaveAngle = 180;
    public WeaponAnimation animator;

    override public bool Attack(Transform target)
    {
        if (timeToAttack <= 0)
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

        if (knockback)
        {  
            takenDamage = target.GetComponent<AttackingCharacter>().TakeDamageWithKnockback(damage + damageAddition, (target.position - transform.position).normalized, knockbackForce);
        }
        else
        {
            takenDamage = target.GetComponent<AttackingCharacter>().TakeDamage(damage + damageAddition);
        }

        return takenDamage;
    }

    public int AttackCleave(float damageAddition)
    {

        if (timeToAttack > 0)
            return 0;

        if(animator)
            animator.WeaponStrike();

        /*List<Transform> visibleTargets = new List<Transform>();

        float lockRadius = range;

        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, lockRadius, parent.ignoreMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {

            if (targetsInRadius[i].GetComponent<AttackingCharacter>() == null)
                continue;

            Vector2 dirToTarget = (targetsInRadius[i].transform.position - transform.position).normalized;

            if (Vector2.Angle(parent.GetFacingDirection(), dirToTarget) < cleaveAngle / 2)
            {

                float distance = Vector2.Distance(transform.position, targetsInRadius[i].transform.position);

                if (parent.CanSee(targetsInRadius[i].transform, distance))
                {
                    visibleTargets.Add(targetsInRadius[i].transform);
                }
            }

        }*/

        print("in autolock range: " + autolock.enemiesInRange.Count);
        print("in weapon range: " + enemiesInRange.Count);

        int damagedTargets = 0;
        /*for(int i = 0; i < autolock.enemiesInRange.Count; i++)
        {
            target = autolock.enemiesInRange;
            if (target != null && IsInRange(target) && Damage(target, damageAddition))
            {
                damagedTargets++;
            }
        }*/

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

        print("damaged: " + damagedTargets.ToString());

        return damagedTargets;
    }
}
