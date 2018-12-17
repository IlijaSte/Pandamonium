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
    private static int FIRST_LEVEL_BUILD_INDEX = 4;

    public int currentLevel = 0;

    [HideInInspector]
    public string gameMode = "classic";
    [HideInInspector]
    public bool isRunStarted = false;

    [HideInInspector]
    public int coins = 0;

    private int tempCoins = 0;

    [HideInInspector]
    public List<string> abilities;

    [HideInInspector]
    public int[] attributes;

    [HideInInspector]
    public PrefabHolder prefabHolder;

    [HideInInspector]
    public UpgradeManager costHolder;

    public static int NUM_UPGRADES = (int)PlayerWithJoystick.AttributeType.CASH_IN + 1;

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

        if (SaveManager.I.gameState != null)
        {
            isRunStarted = SaveManager.I.gameState.isRunStarted;
            gameMode = SaveManager.I.gameState.gameMode;
            currentLevel = SaveManager.I.gameState.gameLevel;
            coins = SaveManager.I.gameState.coins;
            abilities = SaveManager.I.gameState.abilities;
            attributes = SaveManager.I.gameState.attributes;

            if (attributes == null || attributes.Length < NUM_UPGRADES)
            {
                attributes = new int[NUM_UPGRADES];
            }
        }

        prefabHolder = GetComponent<PrefabHolder>();
        costHolder = GetComponent<UpgradeManager>();

    }

    private void Start()
    {
        
    }

    private void SetupLevel()
    {
        //coins = SaveManager.I.gameState.coins;
        if (UIManager.I != null)
        {
            UIManager.I.coinsText.text = coins.ToString();
            (playerInstance as PlayerWithJoystick).attributes = attributes;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(playerInstance != null)
        {
            if (SaveManager.I.gameState != null)
            {
                SetupLevel();
            }
        }
    }

    public bool CanUpgrade(PlayerWithJoystick.AttributeType attribute)
    {
        return attributes[(int)attribute] < costHolder.maxLevels - 1 && coins >= costHolder.GetNextLevelCost(attribute);
    }

    public void Upgrade(PlayerWithJoystick.AttributeType attribute)
    {
        if(CanUpgrade(attribute))
        {
            coins -= costHolder.GetNextLevelCost(attribute);
            attributes[(int)attribute] ++;
        }
    }

    public void PickupCoins(int amount)
    {
        //coins += amount;
        tempCoins += amount;
        UIManager.I.coinsText.text = (coins + tempCoins).ToString();
    }

    public void OnDeath()
    {
        tempCoins = 0;
        isRunStarted = false;

        SaveManager.I.SaveGame();
    }

    public void GameOver()
    {
        isRunStarted = false;
        SaveManager.I.SaveGame();
        LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel >= NUM_OF_LEVELS) {

            if (gameMode.Equals("classic"))
            {
                currentLevel = 0;
                GameOver();
                return;
            }
        }

        coins += tempCoins;
        tempCoins = 0;
        SaveManager.I.SaveGame();

        // u build settings mora da bude game level za game levelom, redom
        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + Mathf.Clamp(currentLevel, 0, NUM_OF_LEVELS - 1));
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
        LoadSceneLong(sceneName);
    }

    public void SetJoystick(bool joystick)
    {
        GameManager.joystick = joystick;
    }

    private string nextScene;

    public void ChooseGameMode(string gameMode)
    {
        this.gameMode = gameMode;
        LoadScene("CharacterSelection");
    }

    public void OnPlayPressed()
    {
        if(isRunStarted)
        {
            StartGame(currentLevel);
        }
        else
        {
            LoadScene("ModeChoiceScene");
        }
    }

    public void StartGame(int level)
    {
        isRunStarted = true;
        currentLevel = level;

        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + Mathf.Clamp(currentLevel, 0, NUM_OF_LEVELS - 1));
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

    private void OnApplicationQuit()
    {

        if (SaveManager.I != null)
        {
            if(playerInstance == null)
            {
                SaveManager.I.SaveGame(abilities);
            }
            else
            {
                SaveManager.I.SaveGame();
            }
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
