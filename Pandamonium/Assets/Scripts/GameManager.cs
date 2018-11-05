using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {


    private static GameManager instance;
    public static string nextScene;

    public static GameManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    // Use this for initialization
    void Start () {
		if(SceneManager.GetActiveScene().name == "LoadingScene")
        {
            SceneManager.LoadScene(nextScene);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadCharachterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelection");
    }
    
    public void loadTestScene()
    {
        nextScene = "TestScene";
        SceneManager.LoadScene("LoadingScene");
    }
    public void loadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTutorialScene()
    {

        SceneManager.LoadScene("LoadingScene");
        nextScene = "TutorialScene";
       // StartCoroutine(LoadTutorialAsyncScene());


    }

    IEnumerator LoadTutorialAsyncScene()
    { 
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TutorialScene");
    
        while (!asyncLoad.isDone)
        {
         yield return null;
        }
    }
}
