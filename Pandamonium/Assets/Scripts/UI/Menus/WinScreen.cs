using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagment
{
    public class WinScreen : Menu<WinScreen>
    {
        public void OnNextLevelPressed()
        {
            GameManager.I.LoadNextLevel();
            base.OnBackPressed();
        }

        public void OnRestartPressed()
        {
            GameManager.I.LoadSceneLong(SceneManager.GetActiveScene().name);
        }

        public void OnMainMenuPressed()
        {
            GameManager.I.LoadScene("MainMenu");

            // ???
            MainMenu.Open();
        }
    }
}

