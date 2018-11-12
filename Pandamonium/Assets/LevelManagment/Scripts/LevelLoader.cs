using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagment
{
    public class LevelLoader : MonoBehaviour
    {
        private static int _mainMenuIndex = 1;
        
        public static void LoadMainMenuLevel()
        {
            LoadLevel(_mainMenuIndex);
        }

        public static void LoadLevel(string levelName)
        {
            if (Application.CanStreamedLevelBeLoaded(levelName))
            {
                SceneManager.LoadScene(levelName);
            }
            else
            {
                Debug.LogWarning("LEVELLOADER LoadLevel() Error: invalide scene name!");
            }
        }

        public static void LoadLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
            {
                if (levelIndex == _mainMenuIndex)
                {
                    MainMenu.Open();
                }
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                Debug.LogWarning("LEVELLOADER LoadLevel() Error: invalide scene index!");
            }
        }

        public static void ReloadLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LoadLevel(currentScene.name);
        }

        public static void LoadNextLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            int totalSceneCount = SceneManager.sceneCountInBuildSettings;
            int currentSceneIndex = currentScene.buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex == totalSceneCount)
                nextSceneIndex = 1;

            LoadLevel(nextSceneIndex);
        }
    }
}

