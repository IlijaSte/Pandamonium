using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRewardEvent : ShrineEvent {

    public GameObject rewardChestPrefab;

    protected override void Activate()
    {
        base.Activate();

        Complete();
    }

    protected override void Complete()
    {
        base.Complete();

        Transform chestParent = transform.parent.parent.GetComponent<RoomHolder>().chestSpawnPoint;

        Instantiate(rewardChestPrefab, chestParent);
    }
}
