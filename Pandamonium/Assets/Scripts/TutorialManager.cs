using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    
    public GameObject tutorialButtons;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PauseForTutorial(int colliderID)
    {
        switch (colliderID)
        {
            case 0:
                Time.timeScale = 0f;
                break;
            case 1:
                break;
        }
    }
    
}
