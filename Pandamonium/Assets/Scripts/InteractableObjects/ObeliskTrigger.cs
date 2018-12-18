using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskTrigger : MonoBehaviour {


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
        }
    }
}
