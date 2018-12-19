using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : MonoBehaviour {

    public enum PortalType { ENTRY, TO_BOSS, END_OF_LEVEL}

    public PortalType type;

    private Animator animator;

    private bool activated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if(type == PortalType.ENTRY)
        {
            animator.SetTrigger("Activate");
        }

        activated = false;
    }

    public void OnActivated()
    {
        if(animator.speed > 0)      // aktivirao se
        {
            activated = true;

            switch (type)
            {

                case PortalType.TO_BOSS:

                    (GameManager.I.playerInstance as PlayerWithJoystick).Teleport(LevelGeneration.I.bossRoomSpawn);
                    break;

                case PortalType.END_OF_LEVEL:
                    MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
                    activated = false;                                  // !!!
                    break;
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;

            switch (type)
            {
                case PortalType.ENTRY:
                    animator.SetTrigger("Deactivate");
                    break;

                case PortalType.TO_BOSS:
                    animator.SetTrigger("Activate");
                    //player.Teleport(LevelGeneration.I.bossRoomSpawn);
                    break;

                case PortalType.END_OF_LEVEL:
                    animator.SetTrigger("Activate");
                    //MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
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
