using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Enemy {

    public override void Start()
    {
        healthBar = UIManager.I.bossHealthBar;

        base.Start();

        healthBar.gameObject.SetActive(false);

    }

    public override bool TakeDamage(float damage)
    {
        bool takenDamage = base.TakeDamage(damage);
        StartCoroutine(healthBar.FillAmount(health / maxHealth, false));

        if (playerState != PlayerState.ATTACKING)
        {
            //Attack(GameManager.I.playerInstance.transform);
        }
        return takenDamage;
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);
        healthBar.gameObject.SetActive(true);
    }

    protected override void Die()
    {

        room.enemies.Remove(gameObject);

        if (room.enemies.Count == 0)
        {
            InfoText.I.ShowMessage("clear");

            if (room.getRoomHolder().chest != null)
                room.getRoomHolder().chest.GetComponentInChildren<Chest>().Unlock();
        }

        base.Die();

    }

}
