using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public GameObject tutorialButtons;
    // Use this for initialization
    void Start() {
        for (int i = 1; i < 4; i++)
            tutorialButtons.transform.GetChild(i).gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update() {

    }
    public void PauseForTutorial(int colliderID)
    {
        if (colliderID != -1)
        {
            enabled = false;
            print("usao");
            Time.timeScale = 0f;
            CanvasGroup canvasGroup = tutorialButtons.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            tutorialButtons.transform.GetChild(colliderID + 1).gameObject.SetActive(true);
        }
        else
            SceneManager.LoadScene("MainMenu"); 
        

    }

    public void CloseTutorial(int colliderID)
    {
        Time.timeScale = 1f;
        CanvasGroup canvasGroup = tutorialButtons.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        tutorialButtons.transform.GetChild(colliderID).gameObject.SetActive(false);
    }

    public void CloseButtonTutorial()
    {
        CloseTutorial(2);

    }

}
