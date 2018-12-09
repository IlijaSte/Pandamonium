using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosHealtBar : Image
{

    public ChaosHealtBar[] triangles;
  
    protected override void Start()
    {
        base.Start();
        triangles = transform.GetComponentsInChildren<ChaosHealtBar>();
        //print(triangles.Length);
          
    }

    public void FillAmount(float amount)
    {
        //child 0 is self
        int i = 1;

        //if it is not a regular healtbar
        if(triangles != null && triangles.Length > 0)
        {
            //full heal
            if(amount == 1)
            {
                for (; i < triangles.Length; i++)
                    triangles[i].fillAmount = 1;
            }
            else
            {
                //part to lose
                float part = (1 - amount) * triangles.Length;
                while (part > 0 && i < triangles.Length - 2)
                {
                    if(triangles.Length < 17) print(triangles.Length + ", part to lose:" + part);
                    while (part > 1 && i < triangles.Length - 2)
                    {
                        if (triangles.Length < 17) print(triangles.Length + ",triangle" + i + " = 0");
                        triangles[i++].fillAmount = 0;
                       part -= 1 ;
                    }
                    triangles[i].fillAmount = 0.5f;// 1 - part;
                    if (triangles.Length < 17) print(triangles.Length + ",triangle" + i + " = " + (1-part));
                    part = 0;

                }
            }
            

           
        }
        else base.fillAmount = amount;
       

    }
}
	
	

