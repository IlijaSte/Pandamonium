using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : CharacterAnimation
{
    private PlayerWithJoystick player;

    public GameObject[] objectsToDisable;

    private void DisableObjects()
    {
        foreach(GameObject gameObject in objectsToDisable)
        {
            gameObject.SetActive(false);
        }
    }

    private void EnableObjects()
    {
        foreach (GameObject gameObject in objectsToDisable)
        {
            gameObject.SetActive(true);
        }
    }

    protected override void updateVector2()
    {
        if (GameManager.joystick)
        {
            player = GameManager.I.playerInstance as PlayerWithJoystick;
            vector2 = player.GetFacingDirection();
        }
        else base.updateVector2();
    }

    void Update()
    {
        //animation360();
        FlipAnimation();
    }

    protected override void animation360()
    {
        updateVector2();
        
        Vector2 velocity = player.GetComponent<Rigidbody2D>().velocity;
       // print(velocity);
        bool isIdle = Mathf.Approximately(velocity.x, Vector2.zero.x) && Mathf.Approximately(velocity.y, Vector2.zero.y);

        print(isIdle);

        if (isIdle)
        {
            animator.SetLayerWeight(1, 0);
        }
        else
        {
            animator.SetLayerWeight(1, 1);
            updateAngleTo360();
            animator.SetFloat("Angle", angle);
        }
    }
    protected override void FlipAnimation()
    {
        updateVector2();

        Vector2 velocity = player.GetComponent<Rigidbody2D>().velocity;
        // print(velocity);
        bool isIdle = Mathf.Approximately(velocity.x, Vector2.zero.x) && Mathf.Approximately(velocity.y, Vector2.zero.y);

        //print(isIdle);

        if (isIdle)
        {
            //animator.SetLayerWeight(1, 0);
            animator.SetBool("Moving", false);
        }
        else
        {
            /*
            animator.SetLayerWeight(1, 1);
            updateAngleTo360();
            animator.SetFloat("Angle", angle);
            */
            animator.SetBool("Moving", true);
        }

        angle = Vector2.Angle(vector2, new Vector2(1, 0));

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //if ((angle > 0 && angle < 90))

        if (vector2.x < 0)
            sr.flipX = true;
        else if (vector2.x > 0) sr.flipX = false;
    }

    

}

