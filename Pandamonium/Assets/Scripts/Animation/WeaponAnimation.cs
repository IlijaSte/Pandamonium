using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : AnimationMama {

    public PlayerWithJoystick player;
    public GameObject parentWeapon;
    public GameObject cursorIndicator;
    public Weapon weapon;
    public GameObject parentHands;


    private void Update()
    {
        

        updateAngleTo360();
      
        Quaternion rotation = parentWeapon.transform.rotation;
        Vector3 rotationVector = new Vector3(rotation.x, rotation.y, angle);
        rotation = Quaternion.Euler(rotationVector);
        cursorIndicator.transform.rotation = rotation;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        int sortingOrder = player.sprite.sortingOrder;
        sr.sortingOrder = sortingOrder;
        parentHands.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder + 1;

        Vector3 pos = parentWeapon.transform.localPosition ;
        float weaponAngle = angle;

        print(player.facingDirection);
        if (player.facingDirection.x > 0)
        {
            animator.SetBool("FlipX", false);
            if (pos.x > 0)
            {
                parentWeapon.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
                Vector3 vectorHands = parentHands.transform.localPosition;
                parentHands.transform.localPosition = new Vector3(vectorHands.x * -1, vectorHands.y, vectorHands.z);

          
            }
            print("angle: " + weaponAngle);
            if (weaponAngle >= 0 && weaponAngle <= 90)
                weaponAngle = Remap(weaponAngle, 0, 90, 0, 45);
            else weaponAngle = Remap2(weaponAngle, 270, 360, 315, 360);
            print("angle scaled: " + weaponAngle);

        }
        else
        {
            animator.SetBool("FlipX", true);
            if (pos.x < 0)
            {
                parentWeapon.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
                Vector3 vectorHands = parentHands.transform.localPosition;
                parentHands.transform.localPosition = new Vector3(vectorHands.x * -1, vectorHands.y, vectorHands.z);

            
            }
                //weaponAngle -= 180f;

                print("angle: " + weaponAngle);
                weaponAngle = Remap(weaponAngle, 90.0f, 270.0f, 135.0f, 225.0f);
                print("angle scaled: " + weaponAngle);
          

        }
        Vector3 weaponRotationVector = new Vector3(rotation.x, rotation.y, weaponAngle);
        //print(weaponAngle);
       // float rotationScaledZ = Mathf.DeltaAngle(0, weaponAngle);//Vector3.Angle(Vector3.right, weaponRotationVector) / 2;
        //Vector3 weaponRotationVectorScaled = new Vector3(rotation.x, rotation.y, getVectorfrom0To90());
        Quaternion weaponRotation = Quaternion.Euler(weaponRotationVector);
        parentWeapon.transform.rotation = weaponRotation;


    }

    protected void Start()
    {



       // path = transform.parent.GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        animator.SetFloat("SpeedMultiplier", weapon.speed / 1);

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

    protected float getVectorfrom0To90()
    {
        Vector2 vector = player.facingDirection;
        float angle;
        if (player.facingDirection.x > 0)
            angle = Vector2.Angle(Vector2.right, vector);
        else angle = Vector2.Angle(Vector2.left, vector);

        return angle / 2;
    }

    private float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        float fromAbs = from - fromMin;
        float fromMaxAbs = fromMax - fromMin;

        float normal = fromAbs / fromMaxAbs;

        float toMaxAbs = toMax - toMin;
        float toAbs = toMaxAbs * normal;

        float to = toAbs + toMin;

        return to;
    }

    private float Remap2(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normal = Mathf.InverseLerp(fromMin, fromMax, from);
        return Mathf.Lerp(toMin, toMax, normal);
    }
        


}
