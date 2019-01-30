using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityManager : MonoBehaviour {

    public GameObject[] abilityPrefabs;

    protected List<Ability> abilities;

    public AttackingCharacter parent;

    public AutolockTracker autolock;

    public float baseDamage = 10;

    public float globalCooldown = 0.5f;

    [HideInInspector]
    public float globalCDProgress;

    void Start()
    {
        if(parent == null)
        {
            parent = transform.parent.GetComponent<AttackingCharacter>();
        }

        globalCDProgress = globalCooldown;

        abilities = new List<Ability>();

        // MENJATI

        if (SaveManager.I.gameState != null)
        {
            if (GameManager.I.currentLevel > 0)
            {
                foreach (string abName in SaveManager.I.gameState.abilities)
                {
                    foreach (GameObject ability in abilityPrefabs)
                    {
                        Ability ab = ability.GetComponent<Ability>();

                        if (ab.abilityName.Equals(abName))
                        {
                            AddAbility(ab);
                        }
                    }
                }
            }
            else
            {
                AddAbility(abilityPrefabs[0].GetComponent<Ability>());
                AddAbility(abilityPrefabs[1].GetComponent<Ability>());

            }
        }
        else
        {
            AddAbility(abilityPrefabs[0].GetComponent<Ability>());
            AddAbility(abilityPrefabs[1].GetComponent<Ability>());
        }

        GameManager.I.abilities = GetAbilities();

        UpdateAbilityButtons();

        /*foreach (GameObject prefab in abilityPrefabs)
        {
            abilities.Add(Instantiate(prefab, transform).GetComponent<Ability>());
        }*/
    }

    public List<string> GetAbilities()
    {

        List<string> ret = new List<string>();

        foreach(Ability ability in abilities)
        {
            ret.Add(ability.abilityName);
        }

        return ret;
    }

    public void AddAbility(Ability ability)
    {

        Ability newAbility = Instantiate(ability.gameObject, transform).GetComponent<Ability>();

        if (abilities.Count < 3)
        {
            abilities.Add(newAbility);
        }
        else
        {
            abilities[abilities.Count - 1] = newAbility;
        }

        UIManager.I.ChangeAbilitySprite(abilities.Count - 1, newAbility.buttonSprite, newAbility.bgSprite);
    }

    public void AddAbility(Blueprint bp)
    {

        Ability ability;
        bool hasAbility;

        do {

            hasAbility = false;
            ability = bp.GetAbility();

            foreach(Ability ablt in abilities)
            {
                if (ability.GetType().Equals(ablt.GetType()))
                {
                    hasAbility = true;
                    break;
                }
            }

        } while (hasAbility);

        Ability newAbility = Instantiate(ability.gameObject, transform).GetComponent<Ability>();

        if(abilities.Count < 3)
        {
            abilities.Add(newAbility);
        }
        else
        {
            abilities[abilities.Count - 1] = newAbility;
        }

        UIManager.I.ChangeAbilitySprite(abilities.Count - 1, newAbility.buttonSprite, newAbility.bgSprite);
    }

    public void UseAbility(int index)
    {

        if (abilities.Count - 1 < index)
            return;

        if (parent is PlayerWithJoystick)
        {
            if ((parent as PlayerWithJoystick).energy >= abilities[index].manaCost)
            {
                if (!(abilities[index] is ChannelingAbility)) {

                    if (abilities[index].CanCast())
                    {
                        if (abilities[index] is Dash)
                        {
                            if (abilities[index].TryCast(parent.transform.position, parent.GetFacingDirection()))
                            {
                                (parent as PlayerWithJoystick).DecreaseEnergy(abilities[index].manaCost);
                                StartCoroutine(GlobalCooldown());
                            }
                        }
                        /*else
                        {
                            CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>();

                            joystickGroup.alpha = 1;
                        }*/

                    }
                }
                else
                {
                    (abilities[index] as ChannelingAbility).StartChanneling();
                    //CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>();

                    //joystickGroup.alpha = 1;
                }
            }

            if (!(abilities[index] is Dash))
            {

                CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>();

                joystickGroup.alpha = 1;
            }
        }
        else
        {
            abilities[index].TryCast(parent.transform.position, parent.GetFacingDirection());
        }
    }

    public IEnumerator GlobalCooldown()
    {
        globalCDProgress = 0;

        while (globalCDProgress <= globalCooldown)
        {
            globalCDProgress += Time.deltaTime;

            /*foreach (Ability ability in abilities)
            {
                if (ability != null)
                {
                    //UIManager.I.UpdateAbilityCooldown(abilities.IndexOf(ability), globalCDProgress / globalCooldown < ability.cdProgress / ability.cooldown ? globalCDProgress / globalCooldown : ability.cdProgress / ability.cooldown);
                }
            }*/

            yield return null;
        }
    }

    public void StopUsingAbility(int abilityIndex)
    {
        if (abilities.Count - 1 < abilityIndex)
            return;

        CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[abilityIndex].GetComponentInChildren<CanvasGroup>();

        if (abilities[abilityIndex] is ChannelingAbility)
        {
            (abilities[abilityIndex] as ChannelingAbility).StopChanneling();
            //CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[abilityIndex].GetComponentInChildren<CanvasGroup>();

            //joystickGroup.alpha = 0;
        }
        else if (!(abilities[abilityIndex] is Dash))
        {
            //CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[abilityIndex].GetComponentInChildren<CanvasGroup>();

            //joystickGroup.alpha = 0;
            //joystickGroup.blocksRaycasts = false;

            if (abilities[abilityIndex].TryCast(parent.transform.position, joystickGroup.GetComponentInChildren<JoystickController>().lastInputDirection.normalized))
            {
                (parent as PlayerWithJoystick).DecreaseEnergy(abilities[abilityIndex].manaCost);
                StartCoroutine(GlobalCooldown());
            }
        }

        //CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[abilityIndex].GetComponentInChildren<CanvasGroup>();

        joystickGroup.alpha = 0;
    }

    public void DragAbility(int index)
    {
        CanvasGroup joystickGroup = UIManager.I.abilityButtonHolders[index].GetComponentInChildren<CanvasGroup>();

        autolock.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, joystickGroup.GetComponentInChildren<JoystickController>().InputDirection.normalized));
        
        if(abilities[index] != null && abilities[index] is ChannelingAbility)
        {
            (abilities[index] as ChannelingAbility).RotateChannel(joystickGroup.GetComponentInChildren<JoystickController>().InputDirection.normalized);
        }

    }

    public void UpdateAbilityCooldown(Ability ability, float progress)
    {
        if (!abilities.Contains(ability))
            return;

        UIManager.I.UpdateAbilityCooldown(abilities.IndexOf(ability), progress);
    }

    public void UpdateAbilityButtons()
    {
        PlayerWithJoystick player = parent as PlayerWithJoystick;

        int i = 0;

        foreach(Ability ability in abilities)
        {
            if(player.energy < ability.manaCost)
            {
                UIManager.I.DisableAbilityButton(i);
            }
            else
            {
                UIManager.I.EnableAbilityButton(i);
            }
            i++;
        }
    }
}
