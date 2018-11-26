﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{

    private Transform weapon;
    private Transform target;
    private float speed;

    private Transform weaponParent;

    private float damage;
    private float range;

    private bool shot = false;
    public Vector3 direction;

    public bool homing = false;

    private Vector2 startPos;

    public void Shoot(Transform weapon, Transform target, float speed)
    {
        this.weapon = weapon;
        this.target = target;
        direction = (target.position - transform.position).normalized;
        this.speed = speed;
        this.damage = weapon.GetComponent<Weapon>().damage;
        this.range = weapon.GetComponent<Weapon>().range;
        shot = true;
        weaponParent = weapon.parent.parent;

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);
    }

    public void Shoot(Transform weapon, Vector2 direction, float speed)
    {
        this.weapon = weapon;
        this.direction = direction;
        this.speed = speed;

        this.target = null;
        this.damage = weapon.GetComponent<Weapon>().damage;
        this.range = weapon.GetComponent<Weapon>().range;
        homing = false;

        shot = true;

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

    }

    private void Update()
    {
        if (!shot) return;

        if(homing && (target == null || weapon == null))      // ako je target unisten/ubijen u medjuvremenu
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

        if (weapon != null && weaponParent != null && other.gameObject == weaponParent.gameObject)                   // ako je projektil pogodio pucaca
            return;

        AttackingCharacter character = other.GetComponent<AttackingCharacter>();
        Weapon weaponComp = weapon.GetComponent<Weapon>();

        if (target != null && other.transform == target && character != null)                                      // ako je pogodio metu
        {
            if (weaponComp.knockback)
            {
                character.TakeDamageWithKnockback(damage, (other.transform.position - transform.position).normalized, weaponComp.knockbackForce);
            }
            else
            {
                character.TakeDamage(damage);
            }

            if (weaponParent != null)
            {
                if (weaponParent.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
                {
                    if (GameManager.I.playerInstance is PlayerWithJoystick)
                    {
                        (GameManager.I.playerInstance as PlayerWithJoystick).IncreaseEnergy(damage);
                    }
                }
            }

            shot = false;
            Destroy(gameObject);

            return;
        }else if (other.CompareTag("Enemy"))
        {
            if (weapon.GetComponent<Weapon>().knockback)
            {
                character.TakeDamageWithKnockback(damage, (other.transform.position - transform.position).normalized, weaponComp.knockbackForce);
            }
            else
            {
                character.TakeDamage(damage);
            }

            if(weaponParent != null)
            {
                if(weaponParent.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
                {
                    if(GameManager.I.playerInstance is PlayerWithJoystick)
                    {
                        (GameManager.I.playerInstance as PlayerWithJoystick).IncreaseEnergy(damage);
                    }
                }
            }

            shot = false;
            Destroy(gameObject);

            return;
        }

        if (character != null && weapon != null && weaponParent != null && character.type == weaponParent.GetComponent<AttackingCharacter>().type)  // ako je pogodio karaktera istog tipa
        {
            return;
        }

        /*if(other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }*/

    }
    
}