using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    override protected void Attack()
    {
        target.GetComponent<AttackingCharacter>().TakeDamage(damage, target.position - transform.position);
    }
}
