using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimation : CharacterAnimation
{
    AttackingCharacter.PlayerState state;

    protected override void Start()
    {
        base.Start();
        state = transform.parent.GetComponent<Slime>().playerState;
    }

    void Update()
    {

        FlipAnimation();

        
        AttackingCharacter.PlayerState currentState = transform.parent.GetComponent<Slime>().playerState;
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
                case AttackingCharacter.PlayerState.DASHING:
                    animator.SetBool("Dashing", false);
                    break;
            }
            state = currentState;
            switch (state)
            {
                case AttackingCharacter.PlayerState.WALKING:
                case AttackingCharacter.PlayerState.CHASING_ENEMY:
                    animator.SetBool("Walking", true);
                    break;
                case AttackingCharacter.PlayerState.ATTACKING:
                    animator.SetBool("Attacking", true);
                    break;
                case AttackingCharacter.PlayerState.DASHING:
                    animator.SetBool("Dashing", true);
                    break;
                case AttackingCharacter.PlayerState.IMMOBILE:
                    if (transform.parent.GetComponent<Slime>().isDead)
                    {
                        animator.SetBool("Dying", true);
                    }
                    break;
                    

            }
        }

    }


}
