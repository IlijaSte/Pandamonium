using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosHealthBar : Image
{
    enum BarType { HEALTH, POISIOM, ENERGY}

    public float triangleDivider = 10;

    private ChaosHealthBar[] halfTriangles;
    private ChaosHealthBar[] halfTrianglesBG;
    private ChaosHealthBar[] prefabs;

    private Transform foregroundParent;
    private Transform backgroundParent;

    private Color[] colorsBackground;
    private Color[] colorsForeground;
    private Color[] colorsPoison;

    private Vector2 triangleOriginDistances;

    private bool isMini;

    private float healthPeace = 1;
    private float poisonPeace = 0;

    private void SetTriangleOriginDistances()
    { 
        if (isMini)
            triangleOriginDistances = new Vector2(5f, 1f);
        else triangleOriginDistances = new Vector2(11f, 2f);
    }


    private void SetColors()//HealthBarHolder hbh)
    {
        /*
        colorsBackground = hbh.colorsBackground;
        colorsForeground = hbh.colorsForeground;
        colorsPoison = hbh.colorsPoison;
        */
        //float part = (1 - amount) * (halfTriangles.Length - 1);
        float poisonFloatStart = Mathf.Round((healthPeace - poisonPeace) * (halfTriangles.Length));
        //print("poisonFloatStart:" + poisonFloatStart);

        //healht
        for (int i = 0; i < halfTriangles.Length; i += 2)
        {
            float triangleFloatPos = (i + 1);// * halfTriangles.Length;
           // print("triangleFloatPos:" + triangleFloatPos);
            Color32 color;

            if (poisonPeace == 0 || triangleFloatPos < poisonFloatStart)
                color = colorsForeground[(i / 2) % 2];
            else color = colorsPoison[(i / 2) % 2];
            //float part = (1 - amount) * (halfTriangles.Length - 1)

            halfTriangles[i].color = color;

            if (poisonPeace == 0 || triangleFloatPos + 1 < poisonFloatStart)
                color = colorsForeground[(i / 2) % 2];
            else color = colorsPoison[(i / 2) % 2];

            halfTriangles[i + 1].color = color;
        }
    }

    private int GetNumTriangles(float maxHealth)
    {
        return Mathf.RoundToInt(maxHealth / triangleDivider);
    }

    public void BuildHealtBar(float maxHealth, bool isMini)
    {
        this.isMini = isMini;

        int numTriangles = GetNumTriangles(maxHealth);

        SetTriangleOriginDistances();
        //SetColors();

        HealthBarHolder hbh = GetComponent<HealthBarHolder>();
        prefabs = hbh.halfTrianglesPrefabs;
        foregroundParent = hbh.foregroundParent;
        backgroundParent = hbh.backgroundParent;
        colorsBackground = hbh.colorsBackground;
        colorsForeground = hbh.colorsForeground;
        colorsPoison = hbh.colorsPoison;

        //SetColors();


        halfTriangles = new ChaosHealthBar[numTriangles * 2];
        halfTrianglesBG = new ChaosHealthBar[numTriangles * 2];

        InstantiateTriangles(colorsBackground, triangleOriginDistances, halfTrianglesBG, backgroundParent);
        InstantiateTriangles(colorsForeground, triangleOriginDistances, halfTriangles, foregroundParent);

        gameObject.SetActive(!isMini);
    }
    private void InstantiateTriangles(Color[] colors, Vector2 distances, ChaosHealthBar[] halfTriangles, Transform parentObject)
    {
        Vector3 position = Vector3.zero;
        for (int i = 0; i < halfTriangles.Length; i += 2)
        {
            Color32 color = colors[(i / 2) % 2];

            halfTriangles[i] = Instantiate(prefabs[i % 4], parentObject).GetComponent<ChaosHealthBar>();
            halfTriangles[i].transform.localPosition = position;
            halfTriangles[i].color = color;
            
            position.x += distances[0];

            halfTriangles[i + 1] = Instantiate(prefabs[(i + 1) % 4], parentObject).GetComponent<ChaosHealthBar>();
            halfTriangles[i + 1].transform.localPosition = position;
            halfTriangles[i + 1].color = color; 
                                       
            position.x += distances[1];
        }
    }


    public IEnumerator FillAmount(float amount, bool toEnable)
    {
        //healthPeace = amount; 

        if (toEnable)
            gameObject.SetActive(true); 

        //if it is not a regular healtbar
        if (halfTriangles != null && halfTriangles.Length > 0)
        {
            
            //full heal
            if(amount == 1 || amount == 0)
            {
                for (int i = 0; i < halfTriangles.Length; i++)
                    halfTriangles[i].fillAmount = amount;

                print("svaki ovoliko: " + amount);
            }else
            {
                
                int i = halfTriangles.Length - 1;
                //part to lose
                float part = (1 - amount) * (halfTriangles.Length);//proveri ovaj - 1

                //while (part > 0 && i > 0)
                //{
                while (part > 1 && i > 0)
                {
                       //print("halftriangle" + i + "0");
                    halfTriangles[i--].fillAmount = 0;
                    part -= 1 ;
                }
                if (part == 1) halfTriangles[i--].fillAmount = 0;
                else halfTriangles[i--].fillAmount = 1 - part;

                part = 0;
                while (i >= 0)
                halfTriangles[i--].fillAmount = 1;
                //}

            }

            SetColors();
        }
        else base.fillAmount = amount;

        if (toEnable)
        {
            if(halfTriangles[0].fillAmount > 0) yield return new WaitForSeconds(1f);
            gameObject.SetActive(false); 
        }
    }



    private bool ColorON(ChaosHealthBar triangle)
    {
        return triangle.color.Equals(new Color(0, 0, 0, 0));
    }
   
    public void PoisonOn(float poisonDamage, float currentHealth)
    {
        poisonPeace += poisonDamage;
        healthPeace = currentHealth;
        print(poisonPeace);
    }

    public void PoisonOff(float poisonDamage, float currentHealth)
    {
       // healthPeace = currentHealth;
        poisonPeace -= poisonDamage;
        print(poisonPeace);
        SetColors();
        
    }

    public void Heal(float amount)
    {
        if (amount == 1)
        {
            for (int i = 0; i < halfTriangles.Length; i++)
                halfTriangles[i].fillAmount = 1;
        }
        else
        {
            int i = 0;
            float part = amount * halfTriangles.Length;
            while (part > 0)
            {
                while (part > 1)
                {
                    halfTriangles[i++].fillAmount = 1;
                    part -= 1;
                }

                halfTriangles[i++].fillAmount = 1 - part;
                part = 0;

            }
        }

        healthPeace = amount;
        poisonPeace = 0;
    }

}
	
	

