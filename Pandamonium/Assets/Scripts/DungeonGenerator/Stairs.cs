using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour {

    public bool right = true;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();
        if (player != null)
        {
            player.AddRotation(right ? 60 : -60);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();
        if (player != null)
        {
            player.AddRotation(right ? -60 : 60);
        }
    }
}
