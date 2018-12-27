using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StoryTrigger : MonoBehaviour {

    public string[] stories;

    public int level;

    [HideInInspector]
    public bool activated = false;

    protected virtual void Activate()
    {
        StoryBoxManager.I.ShowStory(stories);
        activated = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!StoryBoxManager.I.isShown && !activated && collision.GetComponent<PlayerWithJoystick>() != null)
        {
            if (GameManager.I.currentLevel + 1 == level)
                Activate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!StoryBoxManager.I.isShown && !activated && collision.GetComponent<PlayerWithJoystick>() != null)
        {
            if(GameManager.I.currentLevel + 1 == level)
                Activate();
        }
            
    }

}
