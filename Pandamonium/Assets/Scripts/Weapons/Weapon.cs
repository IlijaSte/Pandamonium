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

    public float range;

    protected bool attacking = false;
    [HideInInspector]
    public float timeToAttack = 0;     // brojac koji se smanjuje u zavisnosti od brzine oruzja, kada dodje do 0 ispali se projektil i vraca se brojac na 1
    protected Transform target;

    protected ArrayList enemiesInRange = new ArrayList();                                                                                                                     //DZO JE SERONJA!!! 09.18.2018. Djole :)

    public AttackingCharacter parent;
    public AutolockTracker autolock;

    protected Vector3 direction;

    protected AbilityManager am;

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = range;


        if(parent == null)
        {
            parent = transform.parent.GetComponent<AttackingCharacter>();
        }

        if(autolock == null)
        {
            autolock = transform.parent.GetComponentInChildren<AutolockTracker>();
        }

        am = (parent is PlayerWithJoystick ? (parent as PlayerWithJoystick).abilityManager : null);
    }

    virtual public void Update()
    {

        if (parent == GameManager.I.playerInstance)
        {
            timeToAttack -= speed * Time.deltaTime;

            /*if(am != null && 1 - timeToAttack > am.globalCDProgress / am.globalCooldown)
            {
                UIManager.I.UpdateAttackCooldown(am.globalCDProgress / am.globalCooldown);
            }
            else
            {
                UIManager.I.UpdateAttackCooldown(1 - timeToAttack);
            }*/
            UIManager.I.UpdateAttackCooldown(1 - timeToAttack);

            if(attacking && timeToAttack <= 0)
            {
                AttackInDirection(direction, true);
            }

        }
        else if (attacking)
        {
            timeToAttack -= speed * Time.deltaTime;
        }

        if (attacking && target != null)
        {
            if (timeToAttack <= 0)
            {
                Attack(target);
                timeToAttack = 1;
            }
        }
        
    }

    virtual public void StartAttacking(Transform target)
    {
        if (!attacking)
        {
            attacking = true;
            timeToAttack = 1f;
            this.target = target;
        }
    }

    virtual public void ContinueAttacking(Transform target)
    {
        if (!attacking)
        {
            attacking = true;
            this.target = target;
        }
    }

    virtual public void Pause()
    {
        attacking = false;
    }

    virtual public void Stop()
    {
        attacking = false;
        timeToAttack = 1;
    }

    public void StartHitting(Vector3 direction)
    {
        this.direction = direction;
        autolock.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction.normalized));
        AttackInDirection(direction, true);
        attacking = true;
    }

    public void UpdateDirection(Vector3 direction)
    {
        this.direction = direction;
        autolock.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction.normalized));
    }

    public void StopHitting()
    {
        attacking = false;
    }

    public virtual bool Attack(Transform target)
    {
        if (timeToAttack <= 0 && (am == null || am.globalCDProgress >= am.globalCooldown))
        {
            timeToAttack = 1;
            if(parent)
                parent.OnWeaponAttack();

            if (am != null)
            {
                StartCoroutine(am.GlobalCooldown());
            }

            return true;
        }
        return false;
    }

    public virtual bool AttackInDirection(Vector2 direction, bool regenMana = false)
    {
        if (timeToAttack <= 0 && (am == null || am.globalCDProgress >= am.globalCooldown))
        {
            timeToAttack = 1;

            if (am != null)
            {
                StartCoroutine(am.GlobalCooldown());
            }

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

        if (enemy || collision.GetComponent<AttackableObject>())
        {
            enemiesInRange.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy || collision.GetComponent<AttackableObject>())
        {
            enemiesInRange.Remove(collision.transform);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if ((enemy || collision.GetComponent<AttackableObject>()) && !enemiesInRange.Contains(collision.transform))
        {
            enemiesInRange.Add(collision.transform);
        }
    }

}
