using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public float damage;
    public float speed;
    public float range;

    protected bool attacking = false;
    protected float timeToAttack = 1;     // brojac koji se smanjuje u zavisnosti od brzine oruzja, kada dodje do 0 ispali se projektil i vraca se brojac na 1
    protected Transform target;

    protected ArrayList enemiesInRange = new ArrayList();                                                                                                                     //DZO JE SERONJA!!! 09.18.2018. Djole :)


    virtual public void StartAttacking(Transform target)
    {
        if (!attacking)
        {
            attacking = true;
            timeToAttack = 1f;
            this.target = target;
        }
    }
    virtual public void Stop()
    {
        attacking = false;
    }

    virtual public void Update()
    {
        if (attacking && target != null)
        {
            timeToAttack -= speed * Time.deltaTime;

            if (timeToAttack <= 0)
            {
                Attack();
                timeToAttack = 1;
            }
        }
    }

    protected abstract void Attack();

    public bool IsInRange(Transform character)
    {
        if (enemiesInRange.Contains(character))
        {
            if(character == null)
            {
                enemiesInRange.Remove(character);
                return false;
            }

            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy)
        {
            enemiesInRange.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy)
        {
            enemiesInRange.Remove(collision.transform);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy && !enemiesInRange.Contains(collision.transform))
        {
            enemiesInRange.Add(collision.transform);
        }
    }

}
