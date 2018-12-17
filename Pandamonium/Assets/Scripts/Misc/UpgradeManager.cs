using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager: MonoBehaviour {

    public int maxLevels = 7;

    private int[][] values;

    public int[] healthValues = { 0, 20, 40, 60, 100, 150, 200 };
    public int[] damageValues = { 0, 20, 40, 60, 100, 150, 200 };
    public int[] critChanceValues = { 5, 10, 15, 20, 30, 40, 50 };
    public int[] critDamageValues = { 25, 35, 45, 55, 70, 90, 120 };
    public int[] cashInValues = { 0, 20, 50, 100, 500, 1000, 2000 };

    public int[] costsByUpgradeLevel = { 0, 400, 1000, 2000, 4000, 8000, 20000 };

    private void Awake()
    {
        values = new int[GameManager.NUM_UPGRADES][];
        values[(int)PlayerWithJoystick.AttributeType.HEALTH] = healthValues;
        values[(int)PlayerWithJoystick.AttributeType.DAMAGE] = damageValues;
        values[(int)PlayerWithJoystick.AttributeType.CRIT_CHANCE] = critChanceValues;
        values[(int)PlayerWithJoystick.AttributeType.CRIT_DAMAGE] = critDamageValues;
        values[(int)PlayerWithJoystick.AttributeType.CASH_IN] = cashInValues;
    }

    public int GetStatAsValue(PlayerWithJoystick.AttributeType attribute)
    {
        return values[(int)attribute][GameManager.I.attributes[(int)attribute]];
    }

    public float GetStatAsMultiplier(PlayerWithJoystick.AttributeType attribute)
    {
        return values[(int)attribute][GameManager.I.attributes[(int)attribute]] / 100f;
    }

    public int GetNextLevelCost(PlayerWithJoystick.AttributeType attribute)
    {
        if (GameManager.I.attributes[(int)attribute] + 1 >= costsByUpgradeLevel.Length)
            return 0;
        else
            return costsByUpgradeLevel[GameManager.I.attributes[(int)attribute] + 1];
        //return 1;
    }
}
