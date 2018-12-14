using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour {

    Button thisButton;

    public string gotoScene;
    public bool longLoad = false;
    public bool isStarter = false;

    private void Start()
    {
        thisButton = GetComponent<Button>();

        if (isStarter)
        {
            thisButton.onClick.AddListener(delegate { GameManager.I.StartGame(); });
        }
        else
        {

            if (longLoad)
            {
                thisButton.onClick.AddListener(delegate { GameManager.I.LoadSceneLong(gotoScene); });
            }
            else
            {
                thisButton.onClick.AddListener(delegate { GameManager.I.LoadScene(gotoScene); });
            }
        }
        
    }
}
