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
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    if(transform.parent.GetComponent<Frogocite>().isJumping)
                        animator.SetBool("Jumping", true);
                    break;

            }
        }
        


    }

}

