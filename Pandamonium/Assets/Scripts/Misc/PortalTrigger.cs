using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : MonoBehaviour {

    public enum PortalType { TO_BOSS, END_OF_LEVEL}

    public PortalType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;

            switch (type)
            {
                case PortalType.TO_BOSS:
                    player.Teleport(LevelGeneration.I.bossRoomSpawn);
                    break;

                case PortalType.END_OF_LEVEL:
                    MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
                    break;
            }

            

        }

    }
}
