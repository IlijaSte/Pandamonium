using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : Popup {

    public override void Start()
    {
        base.Start();
        
    }

    public override void OnPoppedUp()
    {
        base.OnPoppedUp();

        Time.timeScale = 0;
    }

    public void OnResumePressed()
    {
        Time.timeScale = 1;
        Close();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1;

        GameManager.I.LoadScene("MainMenu");
    }

    public void OnEndRunPressed()
    {
        Time.timeScale = 1;

        GameManager.I.GameOver();
    }

}
