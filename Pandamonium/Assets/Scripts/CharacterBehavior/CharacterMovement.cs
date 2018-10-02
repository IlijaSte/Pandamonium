using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// skripta za pracenje odgovarajuceg 'target' 3D objekta u 2D prostoru
public class CharacterMovement : MonoBehaviour {

    public Transform target;

    public float xOffset;
    public float zOffset;

	
	// Update is called once per frame
	void LateUpdate () {

        if(transform != null && target != null)
            transform.localPosition = new Vector3(target.localPosition.x + xOffset, transform.localPosition.y, target.localPosition.z + zOffset);

    }
}
