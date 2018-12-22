using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ProjectileAnimation : AnimationMama {

    
    private FireProjectile fireProjectile;

    protected override void updateVector2()
    {
        vector3 = fireProjectile.direction;
        vector2 = new Vector2(vector3.x, vector3.y);
    }

    void Start ()
    {
        fireProjectile = GetComponent<FireProjectile>();
        animator = GetComponent<Animator>(); 
    }

    void Update () {

        FlipAnimation(); 

    }
}
