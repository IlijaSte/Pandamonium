using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    override public void Attack(Transform target)
    {
        target.GetComponent<AttackingCharacter>().TakeDamage(damage, target.position - transform.position);
    }
}
