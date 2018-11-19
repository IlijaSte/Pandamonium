using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour {

    
    public static int numOfTrails = 0;
    public static float timeToDamage;
    public static AttackingCharacter player;

    public Transform myParent;
    private bool collidesWithPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (numOfTrails == 0)
            {
                timeToDamage = 1;
            }
            numOfTrails++;
            collidesWithPlayer = true;
            player = collision.transform.GetComponent<AttackingCharacter>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            numOfTrails--;
            collidesWithPlayer = false;
        }
    }

    private void Update()
    {
        if(myParent == null)
        {
            if (collidesWithPlayer) numOfTrails--;
            Destroy(gameObject);
        }
    }
}
