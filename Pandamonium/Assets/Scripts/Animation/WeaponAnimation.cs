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
        

        print("angle: " + angle);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Vector3 pos = parentWeapon.transform.localPosition ;
        float weaponAngle = angle;

        if (player.facingDirection.x > 0)
        {
            animator.SetBool("FlipX", false);
            if (pos.x > 0)
            {
                parentWeapon.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
            }

        }
        else
        {
            animator.SetBool("FlipX", true);
            if (pos.x < 0)
            {
                parentWeapon.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
                weaponAngle -= 180f;
            }
                
        }
        Vector3 weaponRotationVector = new Vector3(rotation.x, rotation.y, weaponAngle);
        Quaternion weaponRotation = Quaternion.Euler(weaponRotationVector);
        parentWeapon.transform.rotation = weaponRotation;


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
        if(weapon.timeToAttack <= 0)
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
