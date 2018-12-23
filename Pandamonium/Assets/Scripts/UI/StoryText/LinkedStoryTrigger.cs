using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedStoryTrigger : StoryTrigger {

    public StoryTrigger linkedTo;

    public bool hasFirstText = true;

    [HideInInspector]
    public bool isFirstActivated = false;

    protected override void Activate()
    {

        if (!linkedTo.activated)
        {
            isFirstActivated = true;
            activated = true;
            if (!hasFirstText)
            {
                StoryBoxManager.I.ShowStory(linkedTo.stories);
            }
            else
            {
                StoryBoxManager.I.ShowStory(stories);
            }
        } else {

            if (!activated)
            {
                isFirstActivated = false;
                activated = true;

                if (!hasFirstText)
                {
                    StoryBoxManager.I.ShowStory(stories);
                }
                else
                {
                    StoryBoxManager.I.ShowStory(linkedTo.stories);
                }

            }else if (isFirstActivated)
            {
                if (hasFirstText)
                {
                    StoryBoxManager.I.ShowStory(stories);
                }
                else
                {
                    StoryBoxManager.I.ShowStory(linkedTo.stories);
                }
            }
            else
            {
                if (!hasFirstText)
                {
                    StoryBoxManager.I.ShowStory(stories);
                }
                else
                {
                    StoryBoxManager.I.ShowStory(linkedTo.stories);
                }
            }
        }
        //base.Activate();
    }
}
