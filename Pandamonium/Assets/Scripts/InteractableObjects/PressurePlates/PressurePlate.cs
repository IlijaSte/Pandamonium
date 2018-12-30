using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {

    private bool activated = false;

    private enum PlateState { DEACTIVATED, ACTIVATING, ACTIVATED, DEACTIVATING}
    private PlateState state;

    public int id;

    private bool standingOn = false;

    private void StartActivating()
    {
        GetComponent<Animator>().SetTrigger("StartActivating");
        state = PlateState.ACTIVATING;
    }

    public void Activate()
    {
        GetComponent<Animator>().SetTrigger("Activate");
        activated = true;
        state = PlateState.ACTIVATED;
    }

    public void FullyPressed()
    {
        transform.parent.parent.GetComponent<PuzzleRoom>().TouchIndicator(id);
    }

    public void Deactivate()
    {
        if(state != PlateState.DEACTIVATED && !standingOn)
            GetComponent<Animator>().SetTrigger("Deactivate");

        activated = false;
        state = PlateState.DEACTIVATED;         // !!!
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerWithJoystick>() != null)
        {
            if(state == PlateState.DEACTIVATED)
                StartActivating();

            standingOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerWithJoystick>() != null)
        {
            standingOn = false;

            if(state == PlateState.DEACTIVATED)
                GetComponent<Animator>().SetTrigger("Deactivate");
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (state == PlateState.DEACTIVATED && collision.GetComponent<PlayerWithJoystick>() != null)
        {
            StartActivating();
        }
    }*/
}
