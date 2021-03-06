﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Worm : StandardEnemy {

    private static int NUM_STATES = 4;

    private float timeToStateChange = 1;

    //public new float speed = 1;

    public GameObject projectilePrefab;
    public GameObject indicatorPrefab;
    public TileBase destructedTile;

    public enum WormState{ BURIED, EMERGING, ATTACKING, BURYING };

    [HideInInspector]
    public WormState state = WormState.BURIED;

    private bool spottedPlayer = false;

    private Animator animator;

    private Vector2 targetPos;

    public override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();

        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);

        transform.position = tilePos + new Vector3(0.5f, 0.5f);
    }

    private void FireProjectiles()
    {

        for (int i = 0; i < 3; i++) {

            Vector2 shootPos;
            Vector2 checkPos;

            int it = 0;
            do {
                shootPos = (Vector2)targetPos + Random.insideUnitCircle;
                checkPos = new Vector2(Mathf.FloorToInt(shootPos.x), Mathf.FloorToInt(shootPos.y));
            } while (++it < 100 && !room.IsTileWalkable(room.groundTilemap, checkPos));

            if (it < 100)
            {
                AcidProjectile projectile = Instantiate(projectilePrefab, (Vector2)transform.position + new Vector2(0, 1.5f), Quaternion.identity).GetComponent<AcidProjectile>();

                Transform indicator = Instantiate(indicatorPrefab, shootPos, Quaternion.identity).transform;

                projectile.Shoot(this, shootPos, indicator);
            }
        }

    }

    public override bool TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        return TakeDamage(damage);
    }

    private Vector3 FindEmergePosition()
    {

        bool hitSmth = false;
        Vector2 emergePos;
        int tries = 0;

        do
        {
            emergePos = room.GetRandomPos();
            
            /*if (tilePos.x < BoardCreator.I.columns && tilePos.y < BoardCreator.I.rows &&
                tilePos.x >= 0 && tilePos.y >= 0 && BoardCreator.I.tiles[tilePos.x][tilePos.y] == BoardCreator.TileType.Floor)
            {*/
            RaycastHit2D hit2D;
            hitSmth = false;

            if (hit2D = Physics2D.Raycast(new Vector2(emergePos.x + 0.5f, emergePos.y + 0.5f), Vector2.zero, 0f, ignoreMask))
            {

                if (hit2D.collider.gameObject.layer == LayerMask.NameToLayer("Characters"))
                {
                    hitSmth = true;
                }

            }
            /*}
            else
            {
                hitSmth = true;
            }*/
            if (!hitSmth && !LevelGeneration.I.IsTileFree(emergePos))
                hitSmth = true;

            if (tries < 80 && Vector3.Distance(emergePos, player.position) > 6)
                hitSmth = true;

            tries++;
        } while (hitSmth);

        return emergePos;

    }

    public void Emerge()
    {
        room.PlaceDetail(transform.position, destructedTile);
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponentInChildren<Canvas>().enabled = true;

    }

    public void Submerge()
    {

        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<Canvas>().enabled = false;

    }

    public override void Attack(Transform target)
    {

    }

    protected void StartSubmerging()
    {
        nextAttackBG.SetActive(false);
    }

    protected override void Update()
    {

        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        if (isDead)
            return;

        if(!spottedPlayer){

            if (CanSee(player, vision.GetComponent<CircleCollider2D>().radius) &&
                (room == LevelGeneration.I.GetRoomAtPos(player.transform.position)))
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

        if (timeToStateChange <= 0)                 // if state needs to change
        {

            timeToStateChange = 1;

            switch (state)
            {

                case WormState.BURIED:

                    if (room != LevelGeneration.I.GetRoomAtPos(player.transform.position))
                    {
                        spottedPlayer = false;
                        return;
                    }

                    //Emerge();

                    break;

                case WormState.EMERGING:

                    nextAttackBG.SetActive(true);
                    targetPos = player.position;

                    break;

                case WormState.ATTACKING:

                    FireProjectiles();
                    StartSubmerging();

                    break;

                case WormState.BURYING:

                    //Submerge();
                    transform.position = FindEmergePosition();

                    UpdateGraph();

                    break;

            }

            if (state != WormState.BURYING && room != LevelGeneration.I.GetRoomAtPos(player.transform.position))
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
