using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent : ShrineEvent {


    protected override void Activate()
    {
        base.Activate();

        LevelGeneration.I.InstantiateEnemiesInRoom(LevelGeneration.I.GetRoomAtPos(transform.position));
    }

    protected override void Complete()
    {
        base.Complete();
    }

}
