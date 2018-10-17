using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : Enemy {

    public float enemySpawnInterval = 5;
    public float spawnRadius = 8;
    public GameObject enemyPrefab;

    private float currInterval = 5;
    private float visionRadius;

    public override void Start()
    {
        base.Start();
        currInterval = enemySpawnInterval;
        visionRadius = vision.GetComponent<CircleCollider2D>().radius;
    }

    protected override void Update()
    {
        base.Update();

        if (CanSee(player, visionRadius))
        {

            currInterval -= Time.deltaTime;

            if (currInterval <= 0)
            {
                currInterval = enemySpawnInterval;

                SpawnEnemy();
            }
        }
        else currInterval = enemySpawnInterval;
    }

    private Vector2 GetValidSpawnPoint()
    {
        bool hitSmth = false;
        Vector2 spawnPos;
        do
        {
            spawnPos = new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle * spawnRadius;

            RaycastHit2D hit2D;
            hitSmth = false;

            if (hit2D = Physics2D.Raycast(spawnPos, Vector2.zero, 0f, ignoreMask))
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

        return spawnPos;
    }

    private void SpawnEnemy()
    {

        Vector2 spawnPos = GetValidSpawnPoint();

        Instantiate(enemyPrefab, spawnPos, enemyPrefab.transform.rotation, GameObject.FindGameObjectWithTag("2DWorld").transform);

    }
}
