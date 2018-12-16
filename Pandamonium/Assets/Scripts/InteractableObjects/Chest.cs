using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject {

    public int coinsToDrop = 5;

    public override void Activate()
    {
        DropCoins();
        base.Activate();
        // spawn coins
    }

    public void DropCoins()
    {
        GameObject coinPrefab = GameManager.I.prefabHolder.coin;

        for(int i = 0; i < coinsToDrop; i++)
        {
            Instantiate(coinPrefab, transform.parent.position, Quaternion.identity);
        }
        
    }

}
