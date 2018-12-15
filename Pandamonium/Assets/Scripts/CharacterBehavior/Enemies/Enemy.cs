using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    protected Transform player;

    public static int numEnemies = 0;
    public static bool areAllEnemiesDead = false;

    public int difficulty = 1;

    public GameObject coinPrefab;
    public GameObject[] dropPrefabs;

    protected Vector2 startPos;

    protected Room room;

    protected bool detectedPlayer = false;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    public override void Start()
    {
        numEnemies ++;

        base.Start();

        type = CharacterType.ENEMY;

        room = LevelGeneration.I.GetRoomAtPos(transform.position);
        startPos = transform.position;

        if (path)
        {
            path.maxSpeed = normalSpeed;
        }

        /*approxPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y)) + new Vector2(0.5f, 0.5f);
        currBounds = new Bounds(approxPosition, Vector3.one);

        GraphUpdateObject guo = new GraphUpdateObject(currBounds)
        {
            updatePhysics = true,
            modifyTag = true,
            setTag = (int)type + 1
        };
        AstarPath.active.UpdateGraphs(guo);*/
        player = GameManager.I.playerInstance.transform;
    }

    public virtual void MoveToPosition(Vector3 pos)
    {
        if (playerState != PlayerState.IMMOBILE)
        {
            StopAttacking();
            playerState = PlayerState.WALKING;
            CM.MoveToPosition(new Vector3(pos.x, pos.y, transform.position.z));
        }
    }

    public virtual void Attack(Transform target)
    {

        if (this.target != null && target == this.target)                     // ako je target razlicit od trenutnog
            return;

        if (playerState == PlayerState.IMMOBILE)
            return;

        this.target = target;
        CM.MoveToPosition(target.position);

        playerState = PlayerState.CHASING_ENEMY;
        weapons[equippedWeaponIndex].Stop();
    }

    protected void DropItem()
    {
        if(Random.Range(0, (float)1) >= 0.95f && dropPrefabs.Length > 0)
            Instantiate(dropPrefabs[Random.Range(0, dropPrefabs.Length)], transform.position, Quaternion.identity);
    }

    protected void DropCoins(int amount = 1)
    {
        Instantiate(coinPrefab, transform.position, Quaternion.identity).GetComponent<Collectible>().SetDropDirection((transform.position - player.position).normalized);
    }

    public void StopAttacking()
    {

        target = null;
        playerState = PlayerState.IDLE;

        if (equippedWeaponIndex < weapons.Length)
            weapons[equippedWeaponIndex].Stop();
    }

    public void StopMoving()
    {
        CM.StopMoving();
    }

    protected override void Update()
    {

        base.Update();

        UpdateGraph();

        switch (playerState)
        {
            case PlayerState.CHASING_ENEMY:                                     // ako trenutno juri protivnika
                {

                    if (target == null)                                         // ako je protivnik mrtav
                    {

                        StopAttacking();
                        StopMoving();
                        detectedPlayer = false;
                        break;
                    }

                    if (CanSee(target))
                    {

                        weapons[equippedWeaponIndex].StartAttacking(target);                  // krece da napada oruzjem
                        playerState = PlayerState.ATTACKING;

                        CM.StopMoving();
                    }
                    else if (!CM.destination.Equals(target.position))          // ako se protivnik u medjuvremenu pomerio
                    {
                        CM.MoveToPosition(target.position);
                    }

                    if (LevelGeneration.I.GetRoomAtPos(target.position) != room)
                    {
                        MoveToPosition(startPos);
                        detectedPlayer = false;
                    }
                    

                    break;
                }

            case PlayerState.ATTACKING:
                {
                    Worm worm;
                    if (target == null || ((worm = target.GetComponent<Worm>()) != null && worm.state == Worm.WormState.BURIED))                                         // ako je protivnik mrtav
                    {
                        StopAttacking();
                        StopMoving();
                        detectedPlayer = false;
                        break;
                    }

                    if (!CanSee(target))
                    {                                      // ako mu je protivnik nestao iz weapon range-a

                        Transform tempTarget = target;
                        StopAttacking();
                        Attack(tempTarget);
                    }
                    if (LevelGeneration.I.GetRoomAtPos(target.position) != room)
                    {
                        MoveToPosition(startPos);
                        detectedPlayer = false;
                    }

                    break;
                }

            case PlayerState.WALKING:
                {

                    if (!path.pathPending && (path.reachedEndOfPath || !path.hasPath))      // ako je stigao do destinacije
                    {
                        playerState = PlayerState.IDLE;
                    }else

                    if (((Vector2)path.destination).Equals(startPos))      // ako se vraca na mesto
                    {
                        Transform closest;

                        if ((closest = vision.GetClosest()) != null && LevelGeneration.I.GetRoomAtPos(closest.position) == room)
                        {
                            Attack(closest);
                        }
                    }



                    break;
                }
            case PlayerState.IDLE:
                {
                    if (!detectedPlayer)
                    {
                        Transform closest;

                        if ((closest = vision.GetClosest()) != null && LevelGeneration.I.GetRoomAtPos(closest.position) == room)
                        {
                            Attack(closest);
                            detectedPlayer = true;
                        }
                    }
                    else
                    {
                        Attack(player);
                    }

                    break;
                }

        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.FillAmount( health / maxHealth);

        if(playerState != PlayerState.ATTACKING)
        {
            //Attack(GameManager.I.playerInstance.transform);
        }
    }

    protected override IEnumerator Death()
    {
        StopAttacking();

        GetComponent<Collider2D>().enabled = false;
        healthBar.transform.parent.gameObject.SetActive(false);
        nextAttackBar.transform.parent.gameObject.SetActive(false);
        sprite.gameObject.SetActive(false);

        if (path)
            path.enabled = false;

        playerState = PlayerState.IMMOBILE;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

    protected override void Die()
    {
        DropCoins();

        numEnemies--;
        if (numEnemies == 0)
            areAllEnemiesDead = true;

        /*GraphUpdateObject guo = new GraphUpdateObject(currBounds)
        {
            updatePhysics = true,
            modifyTag = true,
            setTag = 0
        };
        AstarPath.active.UpdateGraphs(guo);*/

        room.enemies.Remove(gameObject);

        base.Die();
    }

}
