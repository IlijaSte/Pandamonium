using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas joystickCanvas;

    public Text combatTextPrefab;

    public Button attackButton;
    public Button[] abilityButtons;

    private static UIManager instance;

    public static UIManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    private void Start()
    {

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            //joystickCanvas.enabled = false;
        }
    }

    public void ShowHitDamage(Canvas canvas, float yOffset, float damage, bool isPlayer = false)
    {
        Vector2 spawnPos = canvas.transform.position + new Vector3(0, yOffset);

        Transform newObj = Instantiate(combatTextPrefab, spawnPos, Quaternion.identity).transform;
        newObj.SetParent(canvas.transform);
        newObj.localScale = Vector3.one;
        newObj.GetComponent<CombatText>().Show(canvas, damage, (isPlayer ? Color.red : Color.white));
    }

    public void ShowPoisonDamage(Canvas canvas, float yOffset, float damage, bool isPlayer = false)
    {
        Vector2 spawnPos = canvas.transform.position + new Vector3(0, yOffset);

        Transform newObj = Instantiate(combatTextPrefab, spawnPos, Quaternion.identity).transform;
        newObj.SetParent(canvas.transform);
        newObj.localScale = Vector3.one;
        newObj.GetComponent<CombatText>().Show(canvas, damage, Color.green);
    }

    public Button GetAbilityButton(int index)
    {
        return abilityButtons[index];
    }

    public void UpdateAttackCooldown(float progress)
    {
        attackButton.GetComponent<Image>().fillAmount = progress;
    }

    public void UpdateAbilityCooldown(int index, float progress)
    {
        abilityButtons[index].GetComponent<Image>().fillAmount = progress;
    }
}
