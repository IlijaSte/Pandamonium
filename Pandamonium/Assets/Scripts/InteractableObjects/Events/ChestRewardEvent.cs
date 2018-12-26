using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRewardEvent : ShrineEvent {

    public GameObject rewardChestPrefab;

    protected override void Activate()
    {
        Transform chestParent = transform.parent.parent.GetComponent<RoomHolder>().chestSpawnPoint;

        Instantiate(rewardChestPrefab, chestParent);

        base.Activate();
    }
}
