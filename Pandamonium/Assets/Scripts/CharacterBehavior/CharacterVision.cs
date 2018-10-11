using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVision : MonoBehaviour {

    private ArrayList enemiesInRange = new ArrayList();
    private AttackingCharacter character;

    private void Start()
    {
        character = transform.parent.GetComponent<AttackingCharacter>();
    }

    public Transform GetClosest()
    {

        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach(Transform tr in enemiesInRange)
        {
            if (tr == null) continue;           // !!!

            if(character.CanSee(tr, GetComponent<CircleCollider2D>().radius / 2))
            {
                float tempDistance;
                if((tempDistance = Vector3.Distance(tr.position, transform.position)) < minDistance)
                {
                    minDistance = tempDistance;
                    closest = tr;
                }
            }
        }

        return closest;

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
