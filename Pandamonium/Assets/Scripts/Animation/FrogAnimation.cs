using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimation : CharacterAnimation
{
    AttackingCharacter.PlayerState state;

    protected override void Start()
    {
        base.Start();
        state = transform.parent.GetComponent<Frogocite>().playerState;
    }

    void Update()
    {
<<<<<<< HEAD
        FlipAnimation();

        AttackingCharacter.PlayerState currentState = transform.parent.GetComponent<Frogocite>().playerState;
        if(!currentState.Equals(state))
        {
            state = currentState;
            switch(state)
            {
                case AttackingCharacter.PlayerState.IDLE:
                    animator.SetBool("Walking", false);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.WALKING:
                    animator.SetBool("Walking", true);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
=======
        animation360();
        /*
        FlipAnimation();

        /*
        AttackingCharacter.PlayerState currentState = transform.parent.GetComponent<Frogocite>().playerState;
        if(!currentState.Equals(state))
        {
            switch(state)
            {
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", false);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    animator.SetBool("Jumping", false);
                    break;
            }
            state = currentState;
            switch (state)
            {
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", true);
>>>>>>> 8cab54234c1119a33c0ff16551c31fc49379d06d
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
<<<<<<< HEAD
                    if(transform.parent.GetComponent<Frogocite>().isJumping)
                        animator.SetBool("Jumping", true);
=======
                    if (transform.parent.GetComponent<Frogocite>().isJumping)
                    {
                        animator.SetBool("Jumping", true);
                    }
                    if (transform.parent.GetComponent<Frogocite>().isDead)
                    {
                        animator.SetBool("Dying", true);
                    }
>>>>>>> 8cab54234c1119a33c0ff16551c31fc49379d06d
                    break;

            }
        }
<<<<<<< HEAD
        

=======

        /*
         * case AttackingCharacter.PlayerState.IDLE:
                    animator.SetBool("Walking", false);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", true);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", false);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Walking", false);
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    if (transform.parent.GetComponent<Frogocite>().isJumping)
                    {
                        animator.SetBool("Walking", false);
                        animator.SetBool("Attacking", false);
                        animator.SetBool("Jumping", true);
                    }
                    if (transform.parent.GetComponent<Frogocite>().isDead)
                    {
                        animator.SetBool("Walking", false);
                        animator.SetBool("Jumping", false);
                        animator.SetBool("Attacking", false);
                        animator.SetBool("Dying", true);
                    }
                    break;
                    */
>>>>>>> 8cab54234c1119a33c0ff16551c31fc49379d06d

    }

}

