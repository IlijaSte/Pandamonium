using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : InteractableObject {

    public float healAmount = 100;

    public Sprite emptySprite;

    private bool healed = false;

    public override void Activate()
    {

        GameManager.I.playerInstance.Heal(50, true);
        if (!canReactivate)
            GetComponent<SpriteRenderer>().sprite = emptySprite;

        base.Activate();

    }
}
