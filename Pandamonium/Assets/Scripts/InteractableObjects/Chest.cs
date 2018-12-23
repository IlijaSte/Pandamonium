using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject {

    public int coinsToDrop = 5;

    [System.NonSerialized]
    public int additionalCoins;

    public bool locked = false;

    public override void Activate()
    {
        DropCoins();
        base.Activate();
        // spawn coins
    }

    public override bool StartActivating()
    {
        if (!locked && !activated)
        {
            GetComponent<Animator>().SetTrigger("Activate");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropCoins()
    {
        GameObject coinPrefab = GameManager.I.prefabHolder.coin;

        for (int i = 0; i < coinsToDrop + additionalCoins; i++)
        {
            Vector2 direction = Vector2.down;

            float addedRotation = Random.Range(-90, 90);

            float a = (Vector2.SignedAngle(Vector2.right, direction) + addedRotation) * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

            direction *= Random.Range(0.5f, 1);

            Instantiate(coinPrefab, transform.parent.position, Quaternion.identity).GetComponent<Collectible>().SetDropDirection(direction);
        }
        
    }

}
