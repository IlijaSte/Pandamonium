using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// primer jednog ranged oruzja
public class FireStaff : RangedWeapon
{

    public GameObject firePrefab;   // prefab projektila (vatre)

    protected override void Attack()
    {
        // kreiranje projektila na mestu nosioca
        GameObject projectile = Instantiate(firePrefab);
        projectile.transform.position = transform.position;

        // ispaljivanje projektila
        projectile.GetComponent<FireProjectile>().Shoot(transform, target, projectileSpeed);
    }

}
