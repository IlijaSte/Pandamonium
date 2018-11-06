using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy {

    private static int NUM_STATES = 4;

    private float timeToStateChange = 1;

    public float speed = 1;

    public GameObject projectilePrefab;

    public enum WormState{ BURIED, EMERGING, ATTACKING, BURYING };

    [HideInInspector]
    public WormState state = WormState.BURIED;

    private bool spottedPlayer = false;

    private Animator animator;

    public override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();

        Vector3Int tilePos = BoardCreator.I.groundTilemap.WorldToCell(transform.position);

        transform.position = tilePos + new Vector3(0.5f, 0.5f);
    }

    private void FireProjectiles()
    {

        for (int i = 0; i < 3; i++) {
            AcidProjectile projectile = Instantiate(projectilePrefab, (Vector2)transform.position + new Vector2(0, 1.5f), Quaternion.identity).GetComponent<AcidProjectile>();
            Vector2 shootPos = (Vector2)player.position + Random.insideUnitCircle;
            projectile.Shoot(shootPos);
        }

    }

    private Vector3 FindEmergePosition()
    {

        bool hitSmth = false;
        Vector2 emergePos;
        Vector3Int tilePos;
        do
        {
            emergePos = new Vector2(player.position.x, player.position.y) + Random.insideUnitCircle * vision.GetComponent<CircleCollider2D>().radius;

            tilePos = BoardCreator.I.obstacleTilemap.WorldToCell(emergePos);

            if (tilePos.x < BoardCreator.I.columns && tilePos.y < BoardCreator.I.rows &&
                tilePos.x >= 0 && tilePos.y >= 0 && BoardCreator.I.tiles[tilePos.x][tilePos.y] == BoardCreator.TileType.Floor)
            {
                RaycastHit2D hit2D;
                hitSmth = false;

                if (hit2D = Physics2D.Raycast(new Vector2(tilePos.x, tilePos.y), Vector2.zero, 0f, ignoreMask))
                {

                    if (hit2D.transform.CompareTag("Enemy"))
                    {
                        hitSmth = true;

                    }

                }
            }
            else
            {
                hitSmth = true;
            }

            
        } while (hitSmth);

        return tilePos + new Vector3(0.5f, 0.5f);

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
                    //animator.GetComponent<SpriteRenderer>().enabled = true;
                    GetComponentInChildren<Canvas>().enabled = true;

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
                    transform.position = FindEmergePosition();
                    //animator.GetComponent<SpriteRenderer>().enabled = false;
                    break;

            }

            state = (WormState)(((int)state + 1) % NUM_STATES);
            animator.SetInteger("State", (int)state);
        }

    }

}
