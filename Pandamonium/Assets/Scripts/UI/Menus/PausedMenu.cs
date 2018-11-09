using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SampleGame;

namespace LevelManagment
{
    public class PausedMenu : Menu<PausedMenu>  {

        //[SerializeField]
        //private int mainMenuSceneBuildIndex = 1;

        public void Start()
        {
            Time.timeScale = 0;
        }

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            base.OnBackPressed();
        }

        public void OnRestartLevelPressed()
        {
            Time.timeScale = 1;
            //if (GameManager.Instance != null)
            //{
            //    GameManager.Instance.ReloadLevel();
            //}
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            LevelLoader.ReloadLevel();
            base.OnBackPressed();
        }

        public void OnMainManuPressed()
        {
            Time.timeScale = 1;
            //SceneManager.LoadScene(mainMenuSceneBuildIndex);
            //if (MenuManager.Instance != null && MainMenu.Instance != null)
            //{
            //    MenuManager.Instance.OpenMenu(MainMenu.Instance);
            //}
            GameManager.I.LoadScene("MainMenu");
            MainMenu.Open();            
        }

        // overriding the back method with application quiSt
        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            Application.Quit();
        }
    }
}

