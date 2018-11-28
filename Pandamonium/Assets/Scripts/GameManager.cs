using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool joystick = true;

    private static GameManager instance;
    public static string nextScene;

    public AttackingCharacter playerInstance;

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
		if(SceneManager.GetActiveScene().name.Equals("LoadingScene"))
        {
            //SceneManager.LoadScene(nextScene);
            StartCoroutine(LoadAsyncScene(nextScene));
        }
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

    public void LoadScene(String scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadSceneLong(String scene)
    {
        nextScene = scene;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadAsyncScene(String scene)
    { 
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        Slider slider = FindObjectOfType<Slider>();

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / .9f);
            slider.value = progress;
            yield return null;
        }
    }

    public void SetJoystick(bool joystick)
    {
        GameManager.joystick = joystick;
    }
}
