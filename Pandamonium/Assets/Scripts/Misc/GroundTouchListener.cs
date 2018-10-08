using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTouchListener : MonoBehaviour
{

    private void OnMouseDown()
    {
        print("clicked!");
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().MoveToPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
