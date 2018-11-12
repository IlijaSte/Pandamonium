using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SampleGame;
using UnityEngine.SceneManagement;

namespace LevelManagment
{
    public class WinScreen : Menu<WinScreen>
    {
        public void OnNextLevelPressed()
        {
            //Time.timeScale = 1;
            //if (GameManager.Instance != null)
            //{
            //    GameManager.Instance.LoadNextLevel();
            //}
            base.OnBackPressed();
            LevelLoader.LoadNextLevel();
            //SceneManager.LoadScene(1);
        }

        public void OnRestartPressed()
        {
            //Time.timeScale = 1;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            base.OnBackPressed();
            LevelLoader.ReloadLevel();
        }

        public void OnMainMenuPressed()
        {
            //Time.timeScale = 1;
            //MainMenu.Open();
            LevelLoader.LoadMainMenuLevel();
            MainMenu.Open();
        }
    }
}

