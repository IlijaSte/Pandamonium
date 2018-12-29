using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyStoneDude : Boss {

    [Header("Slime spawning")]
    public float slimeSpawnCastTime = 2;
    public int slimesPerCast = 2;


    [Header("Rampage")]
    public int hitsPerRampage = 4;
    public float oneRampageHitTime = 1;
    public int rocksPerRampageHit = 4;

    [Header("Prefabs")]
    public GameObject slimePrefab;
    public GameObject rockPrefab;

    private Vector2 currPoolPos;

    private int numRampages = 0;

    private enum StoneDudeState { DEFAULT, GOING_TO_POOL, SPAWNING_SLIMES, GOING_TO_CENTER, RAMPAGE}
    private StoneDudeState state;

    private bool rampaging = false;

    public override void Start()
    {
        base.Start();

        room.getRoomHolder().GetComponentInChildren<PortalTrigger>().interactable = false;
    }

    public void GoToPool(Vector2 poolPos)
    {
        MoveToPosition(poolPos);
        currPoolPos = poolPos;

        state = StoneDudeState.GOING_TO_POOL;
    }

    private IEnumerator SpawningSlimes()
    {
        playerState = PlayerState.IMMOBILE;
        state = StoneDudeState.SPAWNING_SLIMES;

        yield return new WaitForSeconds(slimeSpawnCastTime);

        SpawnSlimes();

        playerState = PlayerState.IDLE;
        state = StoneDudeState.DEFAULT;
    }

    private void SpawnSlimes()
    {
        for(int i = 0; i < slimesPerCast; i++)
        {

            Vector2 direction = ((Vector2)transform.position - currPoolPos).normalized;

            Vector2 spawnPos;

            do
            {

                float addedRotation = Random.Range(-90, 90);

                float a = (Vector2.SignedAngle(Vector2.right, direction) + addedRotation) * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

                direction *= Random.Range(2, 5);

                spawnPos = currPoolPos + direction;

            } while (!room.IsTileWalkable(room.groundTilemap, spawnPos));

            GameObject newSlime = Instantiate(slimePrefab, spawnPos, Quaternion.identity);
            newSlime.GetComponent<Enemy>().room = room;

        }
    }

    public void SpawnRocks()
    {

        List<Vector2> takenPositions = new List<Vector2>();

        for(int i = 0; i < rocksPerRampageHit; i++)
        {

            Vector2 landPos;
            do
            {
                landPos = room.GetRandomPos();
            } while (takenPositions.Contains(landPos));

            Vector2 spawnPos = landPos + Vector2.up * 20;

            GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity, room.getRoomHolder().transform);
            newRock.GetComponent<StoneDudeRock>().landPos = landPos;
        }
    }

    private IEnumerator Rampaging()
    {
        playerState = PlayerState.IMMOBILE;
        state = StoneDudeState.RAMPAGE;

        for(int i = 0; i < hitsPerRampage; i++)
        {
            yield return new WaitForSeconds(oneRampageHitTime);

            SpawnRocks();
        }

        playerState = PlayerState.IDLE;
        state = StoneDudeState.DEFAULT;
    }

    protected override void OnReachedDestination()
    {
        base.OnReachedDestination();

        switch (state)
        {
            case StoneDudeState.GOING_TO_POOL:
                StartCoroutine(SpawningSlimes());
                break;

            case StoneDudeState.GOING_TO_CENTER:
                StartCoroutine(Rampaging());
                break;
        }

    }

    private bool StartRampage()
    {
        if (state == StoneDudeState.RAMPAGE || state == StoneDudeState.GOING_TO_CENTER || state == StoneDudeState.SPAWNING_SLIMES)
            return false;

        state = StoneDudeState.GOING_TO_CENTER;

        MoveToPosition(room.getRoomHolder().transform.position);

        return true;
    }

    public override bool TakeDamage(float damage)
    {

        bool ret = base.TakeDamage(damage);

        if (!isDead)
        {
            if(health / maxHealth <= 1 - (numRampages + 1) * 0.25f)
            {

                if (StartRampage())
                {
                    numRampages++;
                }
            }
        }

        return ret;
    }

    public override bool TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        return base.TakeDamage(damage);
    }

    protected override void Die()
    {

        room.getRoomHolder().GetComponentInChildren<PortalTrigger>().interactable = true;

        base.Die();
    }
}
