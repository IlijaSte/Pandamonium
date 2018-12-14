using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : AnimationMama {

    public PlayerWithJoystick player;
    public GameObject parentWeapon;
    public GameObject cursorIndicator;
    public MeleeWeapon weapon;

    private void Update()
    {
        updateAngleTo360();
      
        Quaternion rotation = parentWeapon.transform.rotation;
        Vector3 rotationVector = new Vector3(rotation.x, rotation.y, angle);
        rotation = Quaternion.Euler(rotationVector);
        cursorIndicator.transform.rotation = rotation;
        parentWeapon.transform.rotation = rotation;

        
    }

    protected void Start()
    {
       // path = transform.parent.GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        animator.SetFloat("SpeedMultiplayer", weapon.speed / 1);
        //  vector2Before = path.desiredVelocity;
    }
    protected override void updateVector2() {}

    public void WeaponStrike()
    {
        //  animator.SetLayerWeight(3, 1);
        print(weapon.timeToAttack);
        if(weapon.timeToAttack == 1)
            animator.SetTrigger("WeaponStrike");
    }

    protected override void updateAngleTo360()
    {
        Vector2 vector = player.facingDirection;
        if (vector.y < 0)
            angle = 180 + 180 - Vector2.Angle(vector, new Vector2(1, 0));
        else
            angle = Vector2.Angle(vector, new Vector2(1, 0));
    }
}
