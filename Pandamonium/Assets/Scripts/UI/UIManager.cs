using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas mainCanvas;
    public Canvas joystickCanvas;
    public Canvas minimapCanvas;

    public ChaosHealtBar healthBar;
    public Image energyBar;
    public Text coinsText;

    public Text combatTextPrefab;

    public Button attackButton;
    public Sprite actionButtonSprite;

    public Transform[] abilityButtonHolders;

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

    private Sprite weaponSprite;

    public void ButtonToAction()
    {
        weaponSprite = attackButton.image.sprite;
        attackButton.image.sprite = actionButtonSprite;
    }

    public void ButtonToWeapon()
    {
        attackButton.image.sprite = weaponSprite;
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

    public void ShowHeal(Canvas canvas, float yOffset)
    {
        Vector2 spawnPos = canvas.transform.position + new Vector3(0, yOffset);

        Transform newObj = Instantiate(combatTextPrefab, spawnPos, Quaternion.identity).transform;
        newObj.SetParent(canvas.transform);
        newObj.localScale = Vector3.one;
        newObj.GetComponent<CombatText>().ShowHeal(canvas, Color.cyan);
    }

    public Button GetAbilityButton(int index)
    {
        return abilityButtonHolders[index].GetComponentInChildren<Button>();
    }

    public void UpdateAttackCooldown(float progress)
    {
        attackButton.GetComponent<Image>().fillAmount = progress;
    }

    public void ChangeAbilitySprite(int index, Sprite sprite, Sprite backgroundSprite)
    {
        abilityButtonHolders[index].GetChild(0).GetComponent<Image>().sprite = backgroundSprite;
        abilityButtonHolders[index].GetChild(1).GetComponent<Image>().sprite = sprite;
       
    }

    public void UpdateAbilityCooldown(int index, float progress)
    {
        abilityButtonHolders[index].GetChild(1).GetComponent<Image>().fillAmount = progress;
    }
    
    public void DisableAbilityButton(int index)
    {
        abilityButtonHolders[index].GetChild(1).GetComponent<Image>().enabled = false;
    }

    public void EnableAbilityButton(int index)
    {
        abilityButtonHolders[index].GetChild(1).GetComponent<Image>().enabled = true;
    }
}
