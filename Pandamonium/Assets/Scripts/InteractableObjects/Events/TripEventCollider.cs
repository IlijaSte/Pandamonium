using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TripEventCollider : MonoBehaviour {

    [System.NonSerialized]
    public int id;

    public Color activatedColor;

    private Color startColor;

    private void Start()
    {
        startColor = GetComponent<SpriteRenderer>().color;
    }

    public void Reset()
    {
        GetComponent<SpriteRenderer>().color = startColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerWithJoystick>() != null)
        {
            GetComponent<SpriteRenderer>().color = activatedColor;
            transform.parent.GetComponent<TripEvent>().TouchIndicator(id);
        }
    }
}
