using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    private void Start()
    {
        range = 0;
    }

    override protected void Attack()
    {
        target.GetComponent<AttackingCharacter>().TakeDamage(damage, target.position - transform.position);
    }
}
