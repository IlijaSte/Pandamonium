using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintDropTrigger : MonoBehaviour {

    public GameObject blueprint;

    public void Drop()
    {

        Instantiate(blueprint, transform.position, Quaternion.identity);
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
            //GameManager.I.LevelEnd();
            Drop();
        }
    }
}
