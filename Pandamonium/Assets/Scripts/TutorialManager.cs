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
        
        Time.timeScale = 1f;
        CanvasGroup canvasGroup = tutorialButtons.transform.GetChild(colliderID).GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public void CloseTutorial(int colliderID)
    {
        Time.timeScale = 0f;
        CanvasGroup canvasGroup = tutorialButtons.transform.GetChild(colliderID).GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    public void CloseFirstTutorial()
    {
        CloseTutorial(0);
    }
    
}
