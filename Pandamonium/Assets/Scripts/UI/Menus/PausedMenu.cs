using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagment
{
    public class PausedMenu : Menu<PausedMenu>  {

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

            GameManager.I.LoadSceneLong(SceneManager.GetActiveScene().name);
            base.OnBackPressed();
        }

        public void OnMainManuPressed()
        {
            Time.timeScale = 1;

            GameManager.I.LoadScene("MainMenu");
            MainMenu.Open();            
        }

        // overriding the back method with application quiSt
        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}

