using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour {

    public Text coinsText;
    public Text classNameText;
    public UpgradeButton[] upgradeButtons;
    public ScrollSnapRect scrollRect;

    public Canvas upgradeCanvas;

    public void ResetAllStats()
    {
        for(int i = 0; i < GameManager.I.attributes.Length; i++)
        {
            GameManager.I.attributes[i] = 0;
        }

        UpdateButtons();
    }

    public void AddCoins(int amount = 100)
    {
        GameManager.I.coins += amount;
        UpdateButtons();
    }

    private string[] classNames = { "arcanita", "locked", "locked" };

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

    public void ChangeScreen(Toggle toggle)
    {
        upgradeCanvas.enabled = toggle.isOn;
    }

}
