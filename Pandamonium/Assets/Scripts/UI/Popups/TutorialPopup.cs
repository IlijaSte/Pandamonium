using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPopup : Popup
{
    public override void OnBackPressed()
    {
        base.OnBackPressed();
    }

    public void OnYesPressed()
    {
        SceneChangeButton.tutorial = true;
        GameManager.I.LoadSceneLong("TutorialScene");
    }

    public void OnNoPressed()
    {
        SceneChangeButton.tutorial = true;
        GameManager.I.LoadSceneLong("Level1_1");
    }

    protected override IEnumerator Opening()
    {
        OnPoppedUp();
        yield break;
    }

    protected override IEnumerator Closing()
    {
        Destroy(gameObject);
        yield break;
    }

}