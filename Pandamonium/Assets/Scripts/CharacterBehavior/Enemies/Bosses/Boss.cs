using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {

    public override void Start()
    {
        healthBar = UIManager.I.bossHealthBar;

        base.Start();

        healthBar.gameObject.SetActive(false);
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);
        healthBar.gameObject.SetActive(true);
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

}
