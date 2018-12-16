using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : InteractableObject {

    public GameObject rewardChest;

    public override void Activate()
    {
        // spawn chest...
        if(rewardChest)
            rewardChest.SetActive(true);

        base.Activate();
    }

}
