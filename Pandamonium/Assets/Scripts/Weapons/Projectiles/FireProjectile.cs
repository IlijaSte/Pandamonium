using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{

    private Ability ability;
    private Transform target;
    private float speed;

    private Transform weaponParent;

    private float damage;
    private float range;

    private bool shot = false;
    public Vector3 direction;

    public bool homing = false;

    private Vector2 startPos;

    public void Shoot(Ability ability, Transform target, float speed)
    {
        this.ability = ability;
        this.target = target;
        direction = (target.position - transform.position).normalized;
        this.speed = speed;
        this.damage = ability.damage;
        this.range = ability.range;
        shot = true;
        weaponParent = ability.transform.parent.parent;     // promeniti

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);
    }

    public void Shoot(Ability ability, Vector2 direction, float speed)
    {
        this.ability = ability;
        this.direction = direction;
        this.speed = speed;

        this.target = null;
        this.damage = ability.damage;
        this.range = ability.range;
        homing = false;

        shot = true;

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

    }

    private void Update()
    {
        if (!shot) return;

        if(homing && (target == null || ability == null))      // ako je target unisten/ubijen u medjuvremenu
        {
            homing = false;
            return;
        }

        if (homing)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
            transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, startPos) >= range)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (ability != null && weaponParent != null && other.gameObject == weaponParent.gameObject)                   // ako je projektil pogodio pucaca
            return;

        AttackingCharacter character = other.GetComponent<AttackingCharacter>();

        if (target != null && other.transform == target && character != null)                                      // ako je pogodio metu
        {
            if (ability.knockback)
            {
                character.TakeDamageWithKnockback(damage, (other.transform.position - transform.position).normalized, ability.knockbackForce);
            }
            else
            {
                character.TakeDamage(damage);
            }

            shot = false;
            Destroy(gameObject);

            return;
        }else if (other.CompareTag("Enemy"))
        {
            if (ability.knockback)
            {
                character.TakeDamageWithKnockback(damage, (other.transform.position - transform.position).normalized, ability.knockbackForce);
            }
            else
            {
                character.TakeDamage(damage);
            }

            shot = false;
            Destroy(gameObject);

            return;
        }

        if (character != null && ability != null && weaponParent != null && character.type == weaponParent.GetComponent<AttackingCharacter>().type)  // ako je pogodio karaktera istog tipa
        {
            return;
        }

        /*if(other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }*/

    }
    
}