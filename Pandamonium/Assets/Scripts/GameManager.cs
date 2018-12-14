using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool joystick = true;

    private static GameManager instance;

    [HideInInspector]
    public AttackingCharacter playerInstance;

    private static int NUM_OF_LEVELS = 3;
    private static int FIRST_LEVEL_BUILD_INDEX = 3;

    public int currentLevel = 0;

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

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        SaveManager.I.LoadGame();

    }

    private void OnLevelWasLoaded(int level)
    {
        
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        if (currentLevel >= NUM_OF_LEVELS) currentLevel = 0;
        // u build settings mora da bude game level za game levelom, redom
        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + currentLevel);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
        LoadSceneLong(sceneName);
        //print(SceneManager.GetSceneByBuildIndex(FIRST_LEVEL_BUILD_INDEX + currentLevel).name);
    }

    public void SetJoystick(bool joystick)
    {
        GameManager.joystick = joystick;
    }

    public string nextScene;

    void Start()
    {

        

    }

    public void StartGame()
    {
        if (SaveManager.I.gameState != null)
        {
            currentLevel = SaveManager.I.gameState.gameLevel;
        }
        else
        {
            currentLevel = 0;
        }

        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + currentLevel % NUM_OF_LEVELS);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
        LoadSceneLong(sceneName);
    }

    // ovo nadalje smestiti u scene manager neki

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadSceneLong(string scene)
    {

        nextScene = scene;
        StartCoroutine(LoadAsyncScene("LoadingScene"));
        
    }

    void OnAsyncLoad(string loadedScene)
    {
        if (loadedScene.Equals("LoadingScene"))
        {
            StartCoroutine(LoadAsyncScene(nextScene));
        }
    }

    IEnumerator LoadAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        Slider slider = FindObjectOfType<Slider>();

        while (!asyncLoad.isDone)
        {

            float progress = Mathf.Clamp01(asyncLoad.progress / .9f);
            if(slider)
                slider.value = progress;
            yield return null;
        }

        OnAsyncLoad(scene);
    }
}
