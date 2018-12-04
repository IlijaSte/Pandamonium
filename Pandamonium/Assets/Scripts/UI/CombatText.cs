using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {


    float amount;
    Color color;
    float endY;

    float i = 0;

    Text comp;

    Vector2 startPos;
    Vector2 endPos;
    Vector2 oldPos;
    Color startColor;
    Color endColor;

    RectTransform rect;

    public void Show(Canvas canvas, float damage, Color color)
    {
        this.amount = damage;
        this.color = color;
        i = 0;

        endY = 2;

        comp = GetComponent<Text>();

        comp.text = amount.ToString() + " damage";
    }

    public void ShowHeal(Canvas canvas, Color color)
    {
        this.color = color;
        i = 0;

        endY = 2;

        comp = GetComponent<Text>();

        comp.text = "Max Health";
    }

    private void Start()
    {
        
        comp.color = color;

        Color startColor = new Color(comp.color.r, comp.color.g, comp.color.b);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);

        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        startPos = rect.anchoredPosition;
        endPos = rect.anchoredPosition + new Vector2(0, 7f);

    }

    private void Update()
    {

        if (i < 1)
        {
            i += Time.deltaTime;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, i);
            comp.color = Color.Lerp(comp.color, endColor, Time.deltaTime);
            //transform.position = Camera.main.WorldToScreenPoint((Vector2)holder.position + new Vector2(0, yOffset));
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
