using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDetector : MonoBehaviour {

    public Transform parent;

    private void Start()
    {
        if(parent == null)
        {
            parent = transform;
            while(parent.GetComponent<AttackingCharacter>() == null)        // generalizovati u nekom trenutku
            {
                parent = parent.parent;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                print("CLICK2!");

            }
        }
    }

    public void OnMouseDown()
    {
        print("CLICK!");
        //Player.I.Attack(parent);
    }
}
