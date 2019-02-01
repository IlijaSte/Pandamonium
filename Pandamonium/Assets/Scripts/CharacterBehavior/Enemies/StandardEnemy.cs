using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemy : Enemy {

    public override bool TakeDamage(float damage)
    {
        
        bool ret = base.TakeDamage(damage);
        StartCoroutine(healthBar.FillAmount(health / maxHealth, true));

        return ret;
    }

    protected override void Die()
    {
        if (isDead)
            return;

        base.Die();

        DropCoins();

        if (GameManager.I.currentLevel > 0 && holdsKey)
        {
            DropKey();
        }

        room.enemies.Remove(gameObject);

        if (room != LevelGeneration.I.bossRoom && room.enemies.Count == 0)
        {
            InfoText.I.ShowMessage("clear");

            if (room.getRoomHolder().chest != null)
                room.getRoomHolder().chest.GetComponentInChildren<Chest>().Unlock();
        }
    }
}
