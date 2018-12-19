using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskTrigger : MonoBehaviour {

    public Transform exitPortal;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated)
            return;

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            //MenuManager.I.ShowMenu(MenuManager.I.gameEndMenu);
            transform.parent.GetComponent<Animator>().SetTrigger("Cleanse");
            Camera.main.GetComponent<CameraMovement>().PeekAt(transform.parent.GetComponentInChildren<CinemachineVirtualCamera>());
            Camera.main.GetComponent<CameraMovement>().PeekAt(exitPortal.GetComponentInChildren<CinemachineVirtualCamera>());
            activated = true;
        }
    }
}
