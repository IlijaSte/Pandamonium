using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour {

    Button thisButton;

    public string gotoScene;
    public bool longLoad = false;
    public bool isStarter = false;
    public string action = "";
    public string parameter = "";

    private void Start()
    {
        thisButton = GetComponent<Button>();

        if (isStarter)
        {
            thisButton.onClick.AddListener(delegate { GameManager.I.StartGame(-1); });
        }
        else
        {

            if (longLoad)
            {
                thisButton.onClick.AddListener(delegate { GameManager.I.LoadSceneLong(gotoScene); });
            }
            else
            {
                if (action.Equals("GameMode"))
                {
                    thisButton.onClick.AddListener(delegate { GameManager.I.ChooseGameMode(parameter); });
                }else if (action.Equals("Play"))
                {
                    thisButton.onClick.AddListener(delegate { GameManager.I.OnPlayPressed(); });
                }
                else
                {
                    thisButton.onClick.AddListener(delegate { GameManager.I.LoadScene(gotoScene); });
                }
            }
        }
        
    }
}
