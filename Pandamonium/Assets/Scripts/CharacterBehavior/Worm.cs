using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy {

    private static int NUM_STATES = 5;

    private float timeToStateChange = 1;

    public float speed = 1;

    public GameObject projectilePrefab;

    private enum WormState{ BURIED, EMERGING, ATTACKING, BURYING };

    private WormState state = WormState.BURIED;

    private bool spottedPlayer = false;

    private Animator animator;

    public override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();

    }

    private void FireProjectiles()
    {

        for (int i = 0; i < 3; i++) {
            Instantiate(projectilePrefab, (Vector2)player.position + Random.insideUnitCircle, Quaternion.identity);
        }

    }

    private Vector2 FindEmergePosition()
    {

        bool hitSmth = false;
        Vector2 emergePos;
        do
        {
            emergePos = new Vector2(player.position.x, player.position.y) + Random.insideUnitCircle * vision.GetComponent<CircleCollider2D>().radius;

            RaycastHit2D hit2D;
            hitSmth = false;

            if (hit2D = Physics2D.Raycast(emergePos, Vector2.zero, 0f, ignoreMask))
            {

                if (hit2D.transform.CompareTag("Enemy"))
                {
                    hitSmth = true;

                }
                else if (hit2D.transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                {
                    hitSmth = true;
                }
            }
        } while (hitSmth);

        return emergePos;

    }

    protected override void Update()
    {

        if(!spottedPlayer){

            if (CanSee(player, vision.GetComponent<CircleCollider2D>().radius))
            {
                spottedPlayer = true;
            }
            else return;

        }

        if(state == WormState.ATTACKING)
        {
            nextAttackBar.fillAmount = 1 - timeToStateChange;
        }
        else
        {
            nextAttackBG.SetActive(false);
        }

        timeToStateChange -= Time.deltaTime * speed;

        if (timeToStateChange <= 0)                 // if state is changing
        {

            timeToStateChange = 1;

            switch (state)
            {

                case WormState.BURIED:

                    // play emerging animation
                    GetComponent<BoxCollider2D>().enabled = true;
                    animator.GetComponent<SpriteRenderer>().enabled = true;
                    GetComponentInChildren<Canvas>().enabled = true;
                    transform.position = FindEmergePosition();

                    break;

                case WormState.EMERGING:

                    // play attacking animation
                    nextAttackBG.SetActive(true);
                    break;

                case WormState.ATTACKING:

                    // play bury animation
                    FireProjectiles();
                    nextAttackBG.SetActive(false);
                    break;

                case WormState.BURYING:

                    // disable colliders...
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponentInChildren<Canvas>().enabled = false;
                    animator.GetComponent<SpriteRenderer>().enabled = false;
                    break;

            }

            state = (WormState)(((int)state + 1) % NUM_STATES);
            animator.SetInteger("State", (int)state);
        }

    }

}
