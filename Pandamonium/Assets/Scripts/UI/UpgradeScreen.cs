using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour {

    public Text coinsText;
    public Text classNameText;
    public UpgradeButton[] upgradeButtons;
    public ScrollSnapRect scrollRect;

    private string[] classNames = { "Pandolina", "Marauder", "Arcanita" };

    private void Start()
    {
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        foreach(UpgradeButton button in upgradeButtons)
        {
            button.UpdateButton();
        }

        coinsText.text = GameManager.I.coins.ToString();
    }

    private void Update()
    {
        classNameText.text = classNames[scrollRect.GetNearestPage()];
    }

}
