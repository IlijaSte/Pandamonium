using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardousArea<T> : MonoBehaviour where T : AttackingCharacter  {

    protected float lastDamage;

    protected List<T> enemiesInArea = new List<T>();

    public void OnEnable()
    {
        lastDamage = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        T enemy = null;
        if((enemy = collision.gameObject.GetComponent<T>()) != null)
        {
            enemiesInArea.Add(enemy);
            OnCharacterEnter(enemy);
        }
    }

    protected abstract void OnCharacterEnter(T character);

    protected abstract void OnCharacterExit(T character);

    public virtual void DealDamage(float damage)
    {

        for(int i = 0; i < enemiesInArea.Count; i++)
        {
            T enemy = enemiesInArea[i];

            if (enemy && !enemy.isDead && enemy.IsAttackable())
                enemy.TakeDamage(damage);
        }

        lastDamage = Time.time;
    }

    public void DealDamageWithKnockback(float damage, float force)
    {
        for (int i = 0; i < enemiesInArea.Count; i++)
        {
            T enemy = enemiesInArea[i];

            if (enemy && !enemy.isDead && enemy.IsAttackable())
                enemy.TakeDamageWithKnockback(damage, (enemy.transform.position - transform.position).normalized, force);
        }

        lastDamage = Time.time;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        T enemy = null;

        if ((enemy = collision.gameObject.GetComponent<T>()) != null)
        {
            enemiesInArea.Remove(enemy);
            OnCharacterExit(enemy);
        }
    }
}
