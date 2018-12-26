using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventShrine : InteractableObject {

    public ShrineEvent[] goodEvents;
    public ShrineEvent[] badEvents;

    public override void Activate()
    {
        base.Activate();

        ShrineEvent newEvent = null;

        if(Random.value >= 0.5f)
        {
            newEvent = goodEvents[Random.Range(0, goodEvents.Length)];
        }
        else
        {
            newEvent = badEvents[Random.Range(0, badEvents.Length)];
        }

        if(newEvent != null)
            Instantiate(newEvent, transform);

        //interactable = false;
    }
}
