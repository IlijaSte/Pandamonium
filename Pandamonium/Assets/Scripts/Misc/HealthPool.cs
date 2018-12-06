using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour {

    public float healAmount = 100;

    public Sprite emptySprite;

    private bool healed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (healed) return;

        if(collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            GameManager.I.playerInstance.Heal();
            GetComponent<SpriteRenderer>().sprite = emptySprite;

            healed = true;
        }
    }
}
