using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour {

    public PlayerWithJoystick.ActionChangeType action;

    protected bool activated = false;

    protected virtual void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.parent.position.y * 100);
    }

    public virtual void Activate()
    {
        // action at the end of animation
        activated = true;
    }

    public virtual void StartActivating()
    {
        //if(!activated)
            GetComponent<Animator>().SetTrigger("Activate");
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player)
        {
            player.ActionChange(action, transform);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player)
        {
            player.ActionChange(PlayerWithJoystick.ActionChangeType.SWAP_TO_WEAPON, transform);
        }
    }
}
