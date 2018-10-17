using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ProjectileAnimation : MonoBehaviour {

    
    private Animator animator;
    private Vector3 vector3;
    private float angle;

    // Use this for initialization
    void Start ()
    {
        
        FireProjectile fireProjectile = GetComponent<FireProjectile>();
        vector3 = fireProjectile.direction;

        animator = GetComponent<Animator>();
   
    }

    // Update is called once per frame
    public void OnEnable()
    {
        
    }
    void Update () {

 
        Vector2 vector2D = new Vector2(vector3.x, vector3.y);

        //float angle = Vector3.Angle(vectorDirection, new Vector3(0 ,1,0));
        print(vector2D);

        angle = Vector2.Angle(vector2D, new Vector2(1, 0));

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if ((angle > 0 && angle < 90))
        {
           
            print(angle);
            sr.flipY = true;

        }
        else sr.flipY = false;
 

     

    }
}
