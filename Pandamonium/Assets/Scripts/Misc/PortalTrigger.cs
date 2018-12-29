using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : InteractableObject {

    public enum PortalType { ENTRY, TO_BOSS, END_OF_LEVEL}

    public PortalType type;

    private Animator animator;

    protected override void Start()
    {

        base.Start();

        GetComponentInChildren<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt((transform.position.y - 2) * 100);

        //animator = GetComponent<Animator>();

        animator = GetComponentInChildren<Animator>();

        if (type == PortalType.ENTRY)
        {
            animator.SetTrigger("Activate");
            interactable = false;
        }

        //activated = false;
    }

    public override void Activate()
    {
        base.Activate();

        if (animator.speed > 0)      // aktivirao se
        {
            //activated = true;

            switch (type)
            {

                case PortalType.TO_BOSS:

                    (GameManager.I.playerInstance as PlayerWithJoystick).Teleport(LevelGeneration.I.bossRoomSpawn);

                    break;

                case PortalType.END_OF_LEVEL:
                    MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
                    //activated = false;                                  // !!!
                    break;
            }

        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;

            switch (type)
            {
                case PortalType.ENTRY:
                    animator.SetTrigger("Deactivate");
                    break;

            }

            

        }

    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (!activated)
            return;

        if (collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;

            switch (type)
            {

                case PortalType.TO_BOSS:
                    
                    player.Teleport(LevelGeneration.I.bossRoomSpawn);
                    break;

                case PortalType.END_OF_LEVEL:
                    MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
                    activated = false;                                  // !!!
                    break;
            }
        }
    }*/
}
