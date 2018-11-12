using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy {

    private static int NUM_STATES = 4;

    private float timeToStateChange = 1;

    public float speed = 1;

    public GameObject projectilePrefab;
    public GameObject indicatorPrefab;

    public enum WormState{ BURIED, EMERGING, ATTACKING, BURYING };

    [HideInInspector]
    public WormState state = WormState.BURIED;

    private bool spottedPlayer = false;

    private Animator animator;

    private Room room;

    private Vector2 targetPos;

    public override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();

        Vector3Int tilePos = BoardCreator.I.groundTilemap.WorldToCell(transform.position);

        transform.position = tilePos + new Vector3(0.5f, 0.5f);

        room = Room.GetRoomAtPos(transform.position);
    }

    private void FireProjectiles()
    {

        for (int i = 0; i < 3; i++) {
            AcidProjectile projectile = Instantiate(projectilePrefab, (Vector2)transform.position + new Vector2(0, 1.5f), Quaternion.identity).GetComponent<AcidProjectile>();
            Vector2 shootPos = (Vector2)targetPos + Random.insideUnitCircle;

            Transform indicator = Instantiate(indicatorPrefab, shootPos, Quaternion.identity).transform;

            projectile.Shoot(this, shootPos, indicator);
        }

    }

    private Vector3 FindEmergePosition()
    {

        bool hitSmth = false;
        Vector2 emergePos;
        Vector3Int tilePos;
        do
        {
            //emergePos = new Vector2(player.position.x, player.position.y) + Random.insideUnitCircle * vision.GetComponent<CircleCollider2D>().radius;

            emergePos = room.GetRandomPos();

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

    private void Emerge()
    {

        GetComponent<BoxCollider2D>().enabled = true;
        GetComponentInChildren<Canvas>().enabled = true;

    }

    private void Submerge()
    {

        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<Canvas>().enabled = false;

    }

    protected override void Update()
    {

        if(!spottedPlayer){

            if (CanSee(player, vision.GetComponent<CircleCollider2D>().radius) &&
                (room == Room.GetRoomAtPos(player.transform.position)))
            {
                spottedPlayer = true;
                //state = WormState.BURIED;
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

                    if (room != Room.GetRoomAtPos(player.transform.position))
                    {
                        spottedPlayer = false;
                        return;
                    }

                    Emerge();

                    break;

                case WormState.EMERGING:

                    nextAttackBG.SetActive(true);
                    targetPos = player.position;
                    break;

                case WormState.ATTACKING:

                    FireProjectiles();
                    nextAttackBG.SetActive(false);
                    break;

                case WormState.BURYING:

                    Submerge();
                    transform.position = FindEmergePosition();
                    break;

            }

            if (state != WormState.BURYING && room != Room.GetRoomAtPos(player.transform.position))
            {
                state = WormState.BURYING;
                nextAttackBG.SetActive(false);
            }
            else
            {
                state = (WormState)(((int)state + 1) % NUM_STATES);
            }

            animator.SetInteger("State", (int)state);
        }

    }

}
