using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour {

    public float healAmount = 100;

    public Sprite emptySprite;

    [HideInInspector]
    public bool canReactivate = false;

    private bool healed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!canReactivate && healed) return;

        if(collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            GameManager.I.playerInstance.Heal(50, true);
            if(!canReactivate)
                GetComponent<SpriteRenderer>().sprite = emptySprite;

            healed = true;
        }
    }
}
