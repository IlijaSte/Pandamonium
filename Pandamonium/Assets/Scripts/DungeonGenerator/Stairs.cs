using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour {

    public bool right = true;

    private float angle = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();
        if (player != null)
        {
            player.AddRotation(right ? angle : -angle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();
        if (player != null)
        {
            player.AddRotation(right ? -angle : angle);
        }
    }
}
