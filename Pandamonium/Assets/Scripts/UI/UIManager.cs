using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas mainCanvas;
    public Canvas joystickCanvas;
    public Canvas minimapCanvas;

    public ChaosHealthBar healthBar;
    public ChaosHealthBar energyBar;
    public Text coinsText;

    public Text combatTextPrefab;

    public Button attackButton;
    public Sprite weaponSprite;
    public Sprite actionButtonSprite;

    public Transform[] abilityButtonHolders;

    public ChaosHealthBar bossHealthBar;

    public Image keyImage;

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

    public void ButtonToAction(bool active = true)
    {
        attackButton.image.sprite = actionButtonSprite;

        if (!active)
        {
            attackButton.interactable = false;
        }
    }

    public void ButtonToWeapon()
    {
        attackButton.image.sprite = weaponSprite;
        attackButton.interactable = true;
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
        abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
    }

    public void EnableAbilityButton(int index)
    {
        abilityButtonHolders[index].GetChild(1).GetComponent<Image>().enabled = true;
        abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
    }

    public void AttackPointerDown(JoystickController joystick)
    {
        PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;
        if (player.action == PlayerWithJoystick.ActionType.WEAPON)
        {
            joystick.transform.parent.GetComponent<CanvasGroup>().alpha = 1;
            GameManager.I.playerInstance.GetWeapon().StartHitting(joystick.InputDirection);
        }
        else
        {
            player.DoAction();
        }
    }

    public void AttackPointerUp(JoystickController joystick)
    {
        joystick.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        GameManager.I.playerInstance.GetWeapon().StopHitting();
    }

    public void AttackPointerHold(JoystickController joystick)
    {
        GameManager.I.playerInstance.GetWeapon().UpdateDirection(joystick.InputDirection);
    }

    public void ShowKey()
    {
        keyImage.enabled = true;
    }

    public void HideKey()
    {
        keyImage.enabled = false;
    }

    public void HideUI()
    {
        mainCanvas.GetComponent<CanvasGroup>().alpha = 0;
        joystickCanvas.GetComponent<CanvasGroup>().alpha = 0;
        minimapCanvas.GetComponent<CanvasGroup>().alpha = 0;

        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
    }

    public void ShowUI()
    {
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("UI"));

        minimapCanvas.GetComponent<CanvasGroup>().alpha = 1;
        mainCanvas.GetComponent<CanvasGroup>().alpha = 1;
        joystickCanvas.GetComponent<CanvasGroup>().alpha = 1;
    }
}
