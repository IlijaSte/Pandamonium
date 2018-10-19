using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWeaponScript : MonoBehaviour {

    public Text text;

    private void Start()
    {
        text.text = "Melee";
    }
    public void changeText()
    {

        if (text.text.Equals("Melee"))
            text.text = "Range";
        else
            text.text = "Melee";
    }

    public void UpdateText(Text newText)
    {
        text = newText;
    }

}
