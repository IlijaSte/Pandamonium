using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{

    Transform weapon;
    Transform target;
    float speed;

    private bool shot = false;
    private Vector3 targetPos;

    public bool homing = false;

    public void Shoot(Transform weapon, Transform target, float speed)
    {
        this.weapon = weapon;
        this.target = target;
        targetPos = target.position;
        this.speed = speed;
        shot = true;

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);
    }

    private void Update()
    {
        if (!shot) return;

        if(target == null)      // ako je target unisten/ubijen u medjuvremenu
        {
            Destroy(gameObject);
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
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (weapon == null)
        {                                                 // ako je pucac u toku leta njegovog projektila umro
            Destroy(gameObject);
            return;
        }

        if (other.gameObject == weapon.parent.gameObject)                   // ako je projektil pogodio pucaca
            return;

        if (LayerMask.LayerToName(other.gameObject.layer).Equals("Foreground"))
            return;

        AttackingCharacter character = other.GetComponent<AttackingCharacter>();

        if (other.transform == target && character != null)                                      // ako je pogodio metu
        {
            character.TakeDamage(weapon.GetComponent<Weapon>().damage, (other.transform.position - transform.position).normalized);
            shot = false;
            Destroy(gameObject);
        }else if(character != null && character.type == weapon.parent.GetComponent<AttackingCharacter>().type)  // ako je pogodio karaktera istog tipa
        {
            return;
        }
        else if(other.gameObject.GetComponent<FireProjectile>() != null)    // ako je pogodio drugi projektil
        {
            return;
        }else
        {
            //Destroy(gameObject);
        }

    }
}