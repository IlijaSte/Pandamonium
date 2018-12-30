using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableObject : MonoBehaviour, IAttackable {

    public float maxHealth = 20;
    protected float health;

    public ChaosHealthBar healthBar;

    protected virtual void Start()
    {
        health = maxHealth;
        healthBar.BuildHealtBar(maxHealth, true);
    }

    public bool TakeDamage(float damage)
    {
        UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage);

        if ((health -= damage) <= 0)
        {
            Die();
            return true;
        }

        StartCoroutine(healthBar.FillAmount(health / maxHealth, true));

        return true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
