using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text combatTextPrefab;

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

    public void ShowHitDamage(Canvas canvas, float yOffset, float damage, bool isPlayer = false)
    {
        Vector2 spawnPos = canvas.transform.position + new Vector3(0, yOffset);

        Transform newObj = Instantiate(combatTextPrefab, spawnPos, Quaternion.identity).transform;
        newObj.SetParent(canvas.transform);
        newObj.localScale = Vector3.one;
        newObj.GetComponent<CombatText>().Show(canvas, damage, (isPlayer ? Color.red : Color.white));
    }
}
