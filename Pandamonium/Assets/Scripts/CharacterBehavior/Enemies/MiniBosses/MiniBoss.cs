using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Enemy {

    protected override void Die()
    {
        //base.Die();
        var ptr = typeof(AttackingCharacter).GetMethod("Die").MethodHandle.GetFunctionPointer();
        var baseDie = (Action)Activator.CreateInstance(typeof(Action), this, ptr);
        baseDie();

        room.enemies.Remove(gameObject);

        numEnemies--;
        if (numEnemies == 0)
            areAllEnemiesDead = true;

    }
}
