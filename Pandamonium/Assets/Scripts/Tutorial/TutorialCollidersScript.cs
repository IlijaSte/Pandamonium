using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollidersScript : MonoBehaviour {

    public int colliderID;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if ( collision.transform.CompareTag("Player"))
        {
            print("player");
            GameManager gameManager = GameManager.I;

            //  gameManager.PauseForTutorial(colliderID);

            TutorialManager tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
            tutorialManager.PauseForTutorial(colliderID);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            print("izasao");
            GameManager gameManager = GameManager.I;

            //  gameManager.PauseForTutorial(colliderID);

            TutorialManager tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
            tutorialManager.CloseTutorial(colliderID);

        }
    }
}
