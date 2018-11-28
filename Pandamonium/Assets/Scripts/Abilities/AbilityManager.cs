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

        foreach(GameObject prefab in abilityPrefabs)
        {
            abilities.Add(Instantiate(prefab, transform).GetComponent<Ability>());
        }
    }

    public void UseAbility(int index)
    {
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

    public void StopUsingAblility(int abilityIndex)
    {
        if (abilities[abilityIndex] is ChannelingAbility)
        {
            (abilities[abilityIndex] as ChannelingAbility).StopChanneling();
        }
    }

    public void UpdateAbilityCooldown(Ability ability, float progress)
    {
        UIManager.I.UpdateAbilityCooldown(abilities.IndexOf(ability), progress);
    }
}
