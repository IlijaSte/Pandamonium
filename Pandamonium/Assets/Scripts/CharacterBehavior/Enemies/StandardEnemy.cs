using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemy : Enemy {


    protected override void Die()
    {
        base.Die();

        DropCoins();

        if (GameManager.I.currentLevel > 0 && holdsKey)
        {
            DropKey();
        }

        room.enemies.Remove(gameObject);

        if (room.enemies.Count == 0)
        {
            InfoText.I.ShowMessage("Clear");
        }
    }
}
