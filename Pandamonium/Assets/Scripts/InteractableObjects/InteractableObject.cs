using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour {

    public PlayerWithJoystick.ActionChangeType action;

    [HideInInspector]
    public bool interactable = true;

    protected bool activated = false;

    [HideInInspector]
    public bool canReactivate = false;

    protected virtual void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.parent.position.y * 100);
    }

    public virtual void Activate()
    {
        // action at the end of animation
        activated = true;
        interactable = false;

    }

    public virtual bool StartActivating()
    {
        if (!activated || canReactivate)
        {

            if (!canReactivate)
                (GameManager.I.playerInstance as PlayerWithJoystick).ActionChange(PlayerWithJoystick.ActionChangeType.SWAP_TO_WEAPON, transform);

            activated = true;
            interactable = false;

            if (GetComponentInChildren<Animator>())
                GetComponentInChildren<Animator>().SetTrigger("Activate");
            else
                Activate();

            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player && (interactable || canReactivate))
        {
            player.ActionChange(action, transform);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        PlayerWithJoystick player = collision.GetComponent<PlayerWithJoystick>();

        if (player && (interactable || canReactivate))
        {
            player.ActionChange(PlayerWithJoystick.ActionChangeType.SWAP_TO_WEAPON, transform);
        }
    }
}
