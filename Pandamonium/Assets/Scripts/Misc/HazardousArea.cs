using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardousArea : MonoBehaviour {

    private float lastDamage;

    private List<AttackingCharacter> enemiesInArea = new List<AttackingCharacter>();

    public void OnEnable()
    {
        lastDamage = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = null;
        if(enemy = collision.gameObject.GetComponent<Enemy>())
        {
            enemiesInArea.Add(enemy);
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {

        if (Time.time - lastDamage >= interval)
        {

            foreach (AttackingCharacter enemy in enemiesInArea)
            {
                if(enemy)
                    enemy.TakeDamage(damage);

            }

            lastDamage = Time.time;
        }
    }*/

    public void DealDamage(float damage)
    {

        for(int i = 0; i < enemiesInArea.Count; i++)
        {
            AttackingCharacter enemy = enemiesInArea[i];

            if (enemy && !enemy.isDead)
                enemy.TakeDamage(damage);
        }

    }

    public void DealDamageWithKnockback(float damage, float force)
    {
        for (int i = 0; i < enemiesInArea.Count; i++)
        {
            AttackingCharacter enemy = enemiesInArea[i];

            if (enemy && !enemy.isDead)
                enemy.TakeDamageWithKnockback(damage, (enemy.transform.position - transform.position).normalized, force);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = null;
        if (enemy = collision.gameObject.GetComponent<Enemy>())
        {
            enemiesInArea.Remove(enemy);
        }
    }
}
