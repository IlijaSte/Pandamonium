using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVision : MonoBehaviour {

    private ArrayList enemiesInRange = new ArrayList();
    public AttackingCharacter character;

    private void Start()
    {
        if(character == null)
            character = transform.parent.parent.GetComponent<AttackingCharacter>();
    }

    public Transform GetClosest()
    {

        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach(Transform tr in enemiesInRange)
        {
            if (tr == null) continue;           // !!!

            Collider2D collider = tr.GetComponent<Collider2D>();

            if (collider == null || (collider != null && !collider.enabled)) continue;

            if(character.CanSee(tr, GetComponent<CircleCollider2D>().radius))
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

        if (enemy && enemy.type != character.type)
        {
            enemiesInRange.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy && enemy.type != character.type)
        {
            enemiesInRange.Remove(collision.transform);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AttackingCharacter enemy = collision.GetComponent<AttackingCharacter>();

        if (enemy && enemy.type != character.type && !enemiesInRange.Contains(collision.transform))
        {
            enemiesInRange.Add(collision.transform);
        }
    }

    public bool IsInRange(Transform ac)
    {
        return enemiesInRange.Contains(ac);
    }
}
