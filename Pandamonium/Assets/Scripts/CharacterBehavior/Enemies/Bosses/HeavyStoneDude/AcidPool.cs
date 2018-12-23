using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HeavyStoneDude frogo = collision.GetComponent<HeavyStoneDude>();
        if (frogo != null)
        {
            frogo.GoToPool(transform.position);
        }

    }
}
