using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostboltProjectile : Projectile {

    public float freezeDuration = 3f;

    protected override void OnHitEnemy(AttackingCharacter enemyHit)
    {
        if (enemyHit is StandardEnemy)
        {
            (enemyHit as StandardEnemy).Freeze(freezeDuration);
        }

        base.OnHitEnemy(enemyHit);

    }
}
