using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelManagment;
using UnityEngine.SceneManagement;

public class DeathMenu : Menu<DeathMenu> {

	public void OnRestartPressed()
    {
        //LevelLoader.ReloadLevel();
        GameManager.I.LoadSceneLong(SceneManager.GetActiveScene().name);
        base.OnBackPressed();
    }

    public void OnMainManuPressed()
    {
        //GameManager.I.LoadScene("CharacterSelection");
        GameManager.I.GameOver();
        MainMenu.Open();
    }
}
