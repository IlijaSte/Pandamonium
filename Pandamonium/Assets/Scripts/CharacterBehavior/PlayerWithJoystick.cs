using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerWithJoystick : AttackingCharacter {

    public float baseHealth = 100;
    public float maxEnergy = 100;

    public float baseDamage = 5;

    [HideInInspector]
    public float energy = 0;

    public float lockAngle = 60;

    [HideInInspector]
    public Image energyBar;

    public AbilityManager abilityManager;
    public JoystickController controller;

    [HideInInspector]
    public Vector2 facingDirection = Vector2.down;

    private float addedRotation = 0;

    protected bool keyPickedUp = false;
    public enum ActionChangeType { SWAP_TO_PAW, SWAP_TO_KEY, SWAP_TO_WEAPON }
    public enum ActionType { WEAPON, KEY, PAW }

    protected ActionType action = ActionType.WEAPON;
    protected Transform actionObject;

    public enum AttributeType { HEALTH, DAMAGE, CRIT_CHANCE, CRIT_DAMAGE, CASH_IN }

    public int[] attributes;

    [HideInInspector]
    public float speed;

    [HideInInspector]
    public Action<string> onEnemySlain;

    [HideInInspector]
    public Action onCoinCollected;

    [HideInInspector]
    public bool canMove = true;

    private SoundController soundController;

    public override void Awake()
    {
        if (!GameManager.joystick)
        {
            controller.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else{
            GameManager.I.playerInstance = this;
        }

        QuestManager.I.SetupPlayerDelegates();
    }

    public override void Start()
    {

        healthBar = UIManager.I.healthBar;
        energyBar = UIManager.I.energyBar;
        //energyBar.BuildHealtBar(0, !type.Equals(CharacterType.PLAYER));
        //energyBar.buildHealtBar(10, false);

        attributes = GameManager.I.attributes;

        health = baseHealth + baseHealth *  GameManager.I.costHolder.GetStatAsMultiplier(AttributeType.HEALTH);

        base.Start();
        facingDirection = Vector2.down;

        if (abilityManager == null)
        {
            abilityManager = GetComponentInChildren<AbilityManager>();
        }

        speed = normalSpeed;

        soundController = GetComponent<SoundController>();

    }
    public void AddRotation(float angle)
    {
        addedRotation += angle;
    }

    public void PickupBlueprint(Blueprint bp)
    {
        abilityManager.AddAbility(bp);
    }

    public void PickupCoins(int amount = 1)
    {
        GameManager.I.PickupCoins(amount);
        if(onCoinCollected != null)
            onCoinCollected();
    }

    public void Teleport(Vector2 pos)
    {
        transform.position = pos;
    }

    protected override void Update()
    {
        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack <= 0)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
        }

        //energyBar.FillAmount(energy / maxEnergy, false);

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetKeyUp(KeyCode.K))
            {
                keyPickedUp = true;
            }
        }
    }

    private IEnumerator Stunned(float duration)
    {
        canMove = false;

        yield return new WaitForSeconds(duration);

        canMove = true;
    }

    public void Stun(float duration)
    {
        StartCoroutine(Stunned(duration));
    }

    protected IEnumerator CaptureScreenshotWOUI()
    {
        UIManager.I.HideUI();

        for (int i = 0; i <= 100; i++)
        {

            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Screenshots"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots");
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/Screenshots/Without UI"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots/Without UI");
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/Screenshots/Without UI/capture" + i + ".png"))
            {
                ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/Screenshots/Without UI/capture" + i + ".png", 4);
                break;
            }
        }
        

        yield return new WaitForSeconds(0.25f);

        UIManager.I.ShowUI();
    }

    protected void CaptureScreenshot()
    {

        for (int i = 0; i <= 100; i++)
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Screenshots"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots");
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/Screenshots/With UI"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots/With UI");
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/Screenshots/With UI/capture" + i + ".png"))
            {
                ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/Screenshots/With UI/capture" + i + ".png", 4);
                break;
            }
        }

    }

    protected void FixedUpdate()
    {

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {

            if (playerState != PlayerState.DASHING)
            {

                Vector2 wasdDirection = Vector2.zero;

                if (Input.GetKey(KeyCode.A))
                {
                    wasdDirection += Vector2.left;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    wasdDirection += Vector2.right;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    wasdDirection += Vector2.up;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    wasdDirection += Vector2.down;
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    speed = 30;
                }

                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    speed = normalSpeed;
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    PickupCoins(5000);
                }

                if (canMove && !wasdDirection.Equals(Vector2.zero))
                {
                    wasdDirection = wasdDirection.normalized;

                    if (rb.velocity.magnitude < speed)
                    {
                        if (addedRotation != 0)
                        {
                            if (wasdDirection.x < 0)
                                wasdDirection = Vector2.left;
                            else if(wasdDirection.x == 0)
                            {
                                if(wasdDirection.y > 0 && addedRotation > 0 || wasdDirection.y < 0 && addedRotation < 0)
                                {
                                    wasdDirection = Vector2.right;
                                }
                                else
                                {
                                    wasdDirection = Vector2.left;
                                }
                            }else
                                wasdDirection = Vector2.right;

                            float a = (Vector2.SignedAngle(Vector2.right, wasdDirection) + addedRotation) * Mathf.Deg2Rad;
                            wasdDirection = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                        }
                        rb.AddForce(wasdDirection * speed * 20, ForceMode2D.Force);
                        facingDirection = wasdDirection;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Attack();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                // DASH
                abilityManager.UseAbility(0);
            }

            if(Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Alpha1))
            {
                abilityManager.StopUsingAbility(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                abilityManager.UseAbility(1);
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                abilityManager.StopUsingAbility(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                abilityManager.UseAbility(2);
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                abilityManager.StopUsingAbility(2);
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                ChangeWeapon();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                StartCoroutine(CaptureScreenshotWOUI());
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                CaptureScreenshot();
            }
        }
        else
        {

            if (canMove && (!Mathf.Approximately(controller.InputDirection.x, 0) || !Mathf.Approximately(controller.InputDirection.y, 0)))
            {

                if (rb.velocity.magnitude < speed)
                {
                    facingDirection = controller.InputDirection.normalized;

                    if(addedRotation != 0)
                    {
                        if (facingDirection.x < 0)
                            facingDirection = Vector2.left;
                        else
                            facingDirection = Vector2.right;

                        float a = (Vector2.SignedAngle(Vector2.right, facingDirection) + addedRotation) * Mathf.Deg2Rad;
                        facingDirection = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                    }

                    if (controller.InputDirection.magnitude >= 0.5f)
                    {
                        rb.AddForce(facingDirection.normalized * speed * 20, ForceMode2D.Force);
                    }
                    else
                    {
                        rb.AddForce(facingDirection.normalized * controller.InputDirection.magnitude * speed * 20, ForceMode2D.Force);
                    }
                }

            }
        }
    }

    public void ActionChange(ActionChangeType actionChange, Transform source = null)
    {

        switch (actionChange) {

            case ActionChangeType.SWAP_TO_KEY:

                action = ActionType.KEY;
                if(keyPickedUp)
                    UIManager.I.ButtonToAction(true);
                else
                    UIManager.I.ButtonToAction(false);
                actionObject = source;
                break;

            case ActionChangeType.SWAP_TO_PAW:

                action = ActionType.PAW;
                UIManager.I.ButtonToAction();
                actionObject = source;
                break;

            case ActionChangeType.SWAP_TO_WEAPON:

                action = ActionType.WEAPON;
                UIManager.I.ButtonToWeapon();
                break;

        }

    }

    public override Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    public void IncreaseEnergy(float amount, bool percent = false)
    {
        float newEnergy = (percent ? energy + maxEnergy * (amount / 100f) : energy + amount);

        energy = Mathf.Clamp(newEnergy, 0, maxEnergy);
        abilityManager.UpdateAbilityButtons();

        energyBar.fillAmount = energy / maxEnergy;
    }

    public void DecreaseEnergy(float amount, bool percent = false)
    {
        float newEnergy = (percent ? energy - maxEnergy * (amount / 100f) : energy - amount);

        energy = Mathf.Clamp(newEnergy, 0, maxEnergy);
        abilityManager.UpdateAbilityButtons();

        energyBar.fillAmount = energy / maxEnergy;
    }

    protected void HitWithWeapon()
    {
        bool crit = UnityEngine.Random.value <= GameManager.I.costHolder.GetStatAsMultiplier(AttributeType.CRIT_CHANCE);

        float damage = baseDamage + baseDamage * GameManager.I.costHolder.GetStatAsMultiplier(AttributeType.DAMAGE);

        if (crit)
        {
            damage += baseDamage * GameManager.I.costHolder.GetStatAsMultiplier(AttributeType.CRIT_DAMAGE);
        }

        int roundDamage = Mathf.RoundToInt(damage);

        if (weapons[equippedWeaponIndex] is MeleeWeapon)
        {
            int numHit = ((MeleeWeapon)weapons[equippedWeaponIndex]).AttackCleave(roundDamage);
            IncreaseEnergy(numHit * (roundDamage + weapons[equippedWeaponIndex].damage));
        }
    }

    public override void Attack()
    {
        if (!canMove)
            return;

        if(soundController) soundController.PlaySoundByName("Attack");

        switch (action)
        {
            case ActionType.WEAPON:

                HitWithWeapon();

                break;

            case ActionType.KEY:

                if (keyPickedUp)
                {
                    if (actionObject.GetComponentInChildren<EventShrine>().StartActivating())
                    {
                        UIManager.I.HideKey();
                    }
                }

                break;

            case ActionType.PAW:

                actionObject.GetComponentInChildren<InteractableObject>().StartActivating();
                break;
        }
        

    }

    public void UseAbility(int abilityIndex)
    {
        abilityManager.UseAbility(abilityIndex);

    }

    public void StopUsingAbility(int abilityIndex)
    {
        abilityManager.StopUsingAbility(abilityIndex);
    }

    public override bool TakeDamage(float damage)
    {
        bool takenDamage = false;
        if (!isDead)
        {
            takenDamage = base.TakeDamage(damage);

            StartCoroutine(healthBar.FillAmount(health / maxHealth, false));
            soundController.PlaySoundByName("TakeDamage");
        }

        return takenDamage;
    }

    public override void TakePoisonDamage(float damage)
    {
        
        if (!isDead)
        {
            base.TakePoisonDamage(damage);

            StartCoroutine(healthBar.FillAmount(health / maxHealth, false));

            soundController.PlaySoundByName("TakePoisonDamage");
        }
    }

    public void PickupKey()
    {
        keyPickedUp = true;
        UIManager.I.ShowKey();
    }

    protected override void Die()
    {
        if (!isDead)
        {
            MenuManager.I.ShowMenu(MenuManager.I.deathMenu);
            isDead = true;

            //menjati
            GameManager.I.OnDeath();
            //base.Die();
        }
    }

    protected override IEnumerator Death()
    {
        yield break;
    }

    private void OnApplicationQuit()
    {
        GameManager.I.abilities = abilityManager.GetAbilities();
    }

    private void OnGUI()
    {
        if (!(GetRoom().type == Room.RoomType.GAUNTLET))
            return;

        if (GUI.Button(new Rect(10, 70, 100, 30), "Spawn 3 Slimes"))
        {
            StartCoroutine((LevelGeneration.I as Level1_0Generation).SpawnEnemiesInRoom(GetRoom(), LevelGeneration.I.enemyPrefabs[0], 3));
        }

        if (GUI.Button(new Rect(10, 110, 100, 30), "Spawn 3 Worms"))
        {
            StartCoroutine((LevelGeneration.I as Level1_0Generation).SpawnEnemiesInRoom(GetRoom(), LevelGeneration.I.enemyPrefabs[1], 3));
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Spawn 3 Frogocids"))
        {
            StartCoroutine((LevelGeneration.I as Level1_0Generation).SpawnEnemiesInRoom(GetRoom(), LevelGeneration.I.enemyPrefabs[2], 3));
        }

        if (GUI.Button(new Rect(10, 190, 100, 30), "Spawn 3 Ranged Frogocids"))
        {
            StartCoroutine((LevelGeneration.I as Level1_0Generation).SpawnEnemiesInRoom(GetRoom(), LevelGeneration.I.enemyPrefabs[3], 3));
        }

        if (GUI.Button(new Rect(10, 230, 100, 30), "Spawn Heavy Frogo"))
        {
            StartCoroutine((LevelGeneration.I as Level1_0Generation).SpawnEnemiesInRoom(GetRoom(), LevelGeneration.I.elitePool[0], 1));
        }
    }
}
