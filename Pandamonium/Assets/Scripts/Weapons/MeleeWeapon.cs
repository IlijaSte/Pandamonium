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

    protected void Damage(Transform target)
    {
        if (knockback)
        {
            target.GetComponent<AttackingCharacter>().TakeDamageWithKnockback(damage, (target.position - transform.position).normalized, knockbackForce);
        }
        else
        {
            target.GetComponent<AttackingCharacter>().TakeDamage(damage);
        }
    }

    public int AttackCleave()
    {

        if (timeToAttack > 0)
            return 0;

        if(animator)
            animator.WeaponStrike();

        List<Transform> visibleTargets = new List<Transform>();

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

        }

        
        foreach(Transform target in visibleTargets)
        {
            Damage(target);
        }

        timeToAttack = 1;

        return visibleTargets.Count;
    }
}
