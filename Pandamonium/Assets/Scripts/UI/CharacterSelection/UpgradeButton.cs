using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {

    public PlayerWithJoystick.AttributeType attribute;

    public Text upgradeLevelText;
    public Text costText;

    public void UpdateButton()
    {

        upgradeLevelText.text = GameManager.I.attributes[(int)attribute].ToString();
        costText.text = GameManager.I.costHolder.GetNextLevelCost(attribute).ToString();

        if (GameManager.I.CanUpgrade(attribute))
        {
            costText.color = Color.green;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            if(GameManager.I.attributes[(int)attribute] >= GameManager.I.costHolder.maxLevels - 1)
            {
                costText.text = "Max Level";
            }
            costText.color = Color.red;
            GetComponent<Button>().interactable = false;
        }

        
    }

    public void TryUpgrade()
    {

        GameManager.I.Upgrade(attribute);

        FindObjectOfType<UpgradeScreen>().UpdateButtons();
    }

}
