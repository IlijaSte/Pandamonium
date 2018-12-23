using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StoryTrigger : MonoBehaviour {

    public string[] stories;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerWithJoystick>() != null)
            StoryBoxManager.I.ShowStory(stories);
    }

}
