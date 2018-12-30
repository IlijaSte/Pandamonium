using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutolockTracker : MonoBehaviour {

    public bool autoUpdateRotation = true;

    //[HideInInspector]
    public List<Transform> enemiesInRange = new List<Transform>();

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.GetComponent<AttackingCharacter>() != null || collision.GetComponent<AttackableObject>() != null) && collision.GetComponent<IAttackable>() != GameManager.I.playerInstance as IAttackable)
        {
            if (!enemiesInRange.Contains(collision.transform))
            {
                enemiesInRange.Add(collision.transform);
            }
        }
    }

    public Transform GetClosest()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach(Transform enemy in enemiesInRange)
        {
            float distance;
            if(enemy != null && (distance = Vector2.Distance(transform.position, enemy.position)) < minDistance)
            {
                closest = enemy;
                minDistance = distance;
            }
        }

        return closest;
    }

    private void Update()
    {
        if(autoUpdateRotation)
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, (GameManager.I.playerInstance as PlayerWithJoystick).facingDirection));
       // GetComponent<Rigidbody2D>().rotation = Vector2.SignedAngle(Vector2.right, (GameManager.I.playerInstance as PlayerWithJoystick).facingDirection);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<AttackingCharacter>() != null || collision.GetComponent<AttackableObject>() != null)
        {
            enemiesInRange.Remove(collision.transform);
        }
    }

}
