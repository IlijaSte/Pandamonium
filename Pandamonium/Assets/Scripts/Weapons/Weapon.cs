using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour {

    public float damage;
    public float speed;

    public bool knockback = false;
    public float knockbackForce = 5;

    [HideInInspector]
    public InputField iField;

    [HideInInspector]
    public float range;

    protected bool attacking = false;
    [HideInInspector]
    public float timeToAttack = 1;     // brojac koji se smanjuje u zavisnosti od brzine oruzja, kada dodje do 0 ispali se projektil i vraca se brojac na 1
    protected Transform target;

    protected ArrayList enemiesInRange = new ArrayList();                                                                                                                     //DZO JE SERONJA!!! 09.18.2018. Djole :)

    public AttackingCharacter parent;

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
        timeToAttack = 1;

        if (iField)
        {
            knockbackForce = float.Parse(iField.text);
        }
    }

    private void Start()
    {
        range = GetComponent<CircleCollider2D>().radius;

        if(parent == null)
        {
            parent = transform.parent.parent.GetComponent<AttackingCharacter>();
        }
    }

    virtual public void Update()
    {
        
        timeToAttack -= speed * Time.deltaTime;

        if(GameManager.I.playerInstance == parent)
        {
            UIManager.I.UpdateAttackCooldown(1 - timeToAttack);
        }

        if (attacking && target != null)
        {
            if (timeToAttack <= 0)
            {
                Attack(target);
                timeToAttack = 1;
            }
        }/*else if(GameManager.joystick && timeToAttack > 0)
        {
            timeToAttack -= speed * Time.deltaTime;
        }*/
        
    }

    public virtual bool Attack(Transform target)
    {
        if (timeToAttack <= 0)
        {
            timeToAttack = 1;
            return true;
        }
        return false;
    }

    public virtual bool AttackInDirection(Vector2 direction)
    {
        if (timeToAttack <= 0)
        {
            timeToAttack = 1;
            return true;
        }
        return false;
    }

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
