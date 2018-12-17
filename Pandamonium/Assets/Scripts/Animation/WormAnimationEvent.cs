using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAnimationEvent : MonoBehaviour {

    private Worm parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<Worm>();
    }

    public void Activate(float direction)
    {
        if (direction > 0)
            parent.Emerge();
        else
            parent.Submerge();
    }
}
