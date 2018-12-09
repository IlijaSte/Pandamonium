using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour {

    public GameObject[] abilityPrefabs;

    protected List<Ability> abilities;

    public AttackingCharacter parent;

    void Start()
    {
        if(parent == null)
        {
            parent = transform.parent.GetComponent<AttackingCharacter>();
        }

        abilities = new List<Ability>();

        // MENJATI

        AddAbility(abilityPrefabs[0].GetComponent<Ability>());
        AddAbility(abilityPrefabs[1].GetComponent<Ability>());

        /*foreach (GameObject prefab in abilityPrefabs)
        {
            abilities.Add(Instantiate(prefab, transform).GetComponent<Ability>());
        }*/
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
                    if(abilities[index].TryCast(parent.transform.position, parent.GetFacingDirection()))
                        (parent as PlayerWithJoystick).DecreaseEnergy(abilities[index].manaCost);
                }
                else
                {
                    (abilities[index] as ChannelingAbility).StartChanneling();
                }
            }
        }
        else
        {
            abilities[index].TryCast(parent.transform.position, parent.GetFacingDirection());
        }
    }

    public void StopUsingAbility(int abilityIndex)
    {
        if (abilities.Count - 1 < abilityIndex)
            return;

        if (abilities[abilityIndex] is ChannelingAbility)
        {
            (abilities[abilityIndex] as ChannelingAbility).StopChanneling();
        }
    }

    public void UpdateAbilityCooldown(Ability ability, float progress)
    {
        if (!abilities.Contains(ability))
            return;

        UIManager.I.UpdateAbilityCooldown(abilities.IndexOf(ability), progress);
    }
}
