﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosHealtBar : Image
{
    enum BarType { HEALTH, POISIOM, ENERGY}

    private ChaosHealtBar[] halfTriangles;
    private ChaosHealtBar[] halfTrianglesBG;
    private ChaosHealtBar[] prefabs;

    private Transform foregroundParent;
    private Transform backgroundParent;

    public Color32[] colorsBackground;
    public Color32[] colorsForeground;
    public Color32[] colorsPoison;

    private Vector2 triangleOriginDistances;

    private bool isMini;

    private void SetTriangleOriginDistances()
    { 
        if (isMini)
            triangleOriginDistances = new Vector2(5f, 1f);
        else triangleOriginDistances = new Vector2(11f, 2f);
    }

    private void SetColors()
    {
        colorsForeground = new Color32[2];
        colorsPoison = new Color32[2];
        colorsBackground = new Color32[2];
        colorsBackground[0] = colorsBackground[1] = new Color32(48, 19, 20, 255);
        colorsForeground[0] = new Color32(130, 25, 34, 255);
        colorsForeground[1] = new Color32(165, 30, 35, 255);
        //posion color:
    }

    public void buildHealtBar(int numTriangles, bool isMini)
    {
        this.isMini = isMini;

        SetTriangleOriginDistances();
        //SetColors();

        HealthBarHolder hbh = GetComponent<HealthBarHolder>();
        prefabs = hbh.halfTrianglesPrefabs;
        foregroundParent = hbh.foregroundParent;
        backgroundParent = hbh.backgroundParent;
        

        halfTriangles = new ChaosHealtBar[numTriangles * 2];
        halfTrianglesBG = new ChaosHealtBar[numTriangles * 2];

        InstantiateTriangles(colorsBackground, triangleOriginDistances, halfTrianglesBG, backgroundParent);
        InstantiateTriangles(colorsForeground, triangleOriginDistances, halfTriangles, foregroundParent);
    }
    private void InstantiateTriangles(Color32[] colors, Vector2 distances, ChaosHealtBar[] halfTriangles, Transform parentObject)
    {
        Vector3 position = Vector3.zero;
        for (int i = 0; i < halfTriangles.Length; i += 2)
        {
            Color32 color = colors[(i / 2) % 2];

            halfTriangles[i] = Instantiate(prefabs[i % 4], parentObject).GetComponent<ChaosHealtBar>();
            halfTriangles[i].transform.localPosition = position;
            halfTriangles[i].color = color;
            
            position.x += distances[0];

            halfTriangles[i + 1] = Instantiate(prefabs[(i + 1) % 4], parentObject).GetComponent<ChaosHealtBar>();
            halfTriangles[i + 1].transform.localPosition = position;
            halfTriangles[i + 1].color = color; 
                                       
            position.x += distances[1];
        }
    }


    public void FillAmount(float amount)
    {

        //if it is not a regular healtbar
        if(halfTriangles != null && halfTriangles.Length > 0)
        {
            //full heal
            if(amount == 1)
            {
                for (int i = 0; i < halfTriangles.Length; i++)
                    halfTriangles[i].fillAmount = 1;
            }
            else
            {

                int i = halfTriangles.Length - 1;
                //part to lose
                float part = (1 - amount) * (halfTriangles.Length - 1);
                while (part > 0 && i > 0)
                {
                    while (part > 1 && i > 1)
                    {
                       halfTriangles[i--].fillAmount = 0;
                       part -= 1 ;
                    }
                    halfTriangles[i].fillAmount = 1 - part;
                    part = 0;
                }
            }
        }
        else base.fillAmount = amount;
    }

    private bool ColorON(ChaosHealtBar triangle)
    {
        return triangle.color.Equals(new Color(0, 0, 0, 0));
    }
   

}
	
	

