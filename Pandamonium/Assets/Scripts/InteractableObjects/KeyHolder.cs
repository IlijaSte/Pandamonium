using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour {

    public void Activate()
    {
        // spawn chest...
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player)
        {
            player.ActionChange(PlayerWithJoystick.ActionChangeType.SWAP_TO_KEY);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player)
        {
            player.ActionChange(PlayerWithJoystick.ActionChangeType.SWAP_TO_WEAPON);
        }
    }
}
