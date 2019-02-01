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

    public string enemyName;

    public int difficulty = 1;

    public GameObject[] dropPrefabs;

    protected Vector2 startPos;

    [HideInInspector]
    public Room room = null;

    protected bool detectedPlayer = false;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    [HideInInspector]
    public bool holdsKey = false;

    [HideInInspector]
    public float speed;

    private volatile float freezeDuration = 0;

    private Coroutine freezeCoroutine;

    protected IEnumerator Frozen()
    {
        speed /= 2;
        if (path)
            path.maxSpeed /= 2;

        float dur = 0;

        while (dur < freezeDuration)
        {
            if (isDead)
                yield break;

            StartCoroutine(ColorTransition(Color.blue));
            yield return new WaitForSeconds(1f);
            dur += 1f;
        }


        speed *= 2;

        if (path)
            path.maxSpeed *= 2;

        freezeDuration = 0;
    }

    public void Freeze(float duration)
    {

        freezeDuration += duration;

        if (freezeCoroutine == null)
        {
            freezeCoroutine = StartCoroutine(Frozen());
        }
    }

    public override void Start()
    {
        numEnemies ++;

        base.Start();

        type = CharacterType.ENEMY;

        if(room == null)
            room = LevelGeneration.I.GetRoomAtPos(transform.position);

        startPos = transform.position;

        if (path)
        {
            path.maxSpeed = normalSpeed;
            speed = normalSpeed;
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
        weapons[equippedWeaponIndex].Pause();
    }

    protected void DropKey()
    {
        GameObject keyPrefab = GameManager.I.prefabHolder.key;
        if (holdsKey)
            Instantiate(keyPrefab, transform.position, Quaternion.identity);
    }

    protected void DropItem()
    {
        if(Random.Range(0, (float)1) >= 0.95f && dropPrefabs.Length > 0)
            Instantiate(dropPrefabs[Random.Range(0, dropPrefabs.Length)], transform.position, Quaternion.identity);
    }

    protected void DropCoins(int amount = 3)
    {
        GameObject coinPrefab = GameManager.I.prefabHolder.coin;

        for (int i = 0; i < amount; i++)
        {
            Vector2 direction = (transform.position - player.position).normalized;

            float addedRotation = Random.Range(-45, 45);

            float a = (Vector2.SignedAngle(Vector2.right, direction) + addedRotation) * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

            direction *= Random.Range(0.5f, 1);

            Instantiate(coinPrefab, transform.position, Quaternion.identity).GetComponent<Collectible>().SetDropDirection(direction);
        }
    }

    public virtual void StopAttacking()
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

    protected virtual void StartAttacking(Transform target)
    {
        weapons[equippedWeaponIndex].StartAttacking(target);                  // krece da napada oruzjem
        playerState = PlayerState.ATTACKING;

        CM.StopMoving();
    }

    protected override void Update()
    {

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
                        StartAttacking(target);
                    }
                    else if (!CM.destination.Equals(target.position))          // ako se protivnik u medjuvremenu pomerio
                    {
                        CM.MoveToPosition(target.position);
                    }

                    if (room != null && player.GetComponent<AttackingCharacter>().GetRoom() != room)
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
                    if (room != null && player.GetComponent<AttackingCharacter>().GetRoom() != room)
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
                        OnReachedDestination();
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
                        if(room == player.GetComponent<PlayerWithJoystick>().GetRoom())
                        {
                            Attack(player);
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

        base.Update();
    }

    protected virtual void OnReachedDestination()
    {

    }

    public override bool TakeDamage(float damage)
    {

        if (isDead)
            return false;

        bool takenDamage = base.TakeDamage(damage);

        if(playerState != PlayerState.ATTACKING)
        {
            //Attack(GameManager.I.playerInstance.transform);
        }
        return takenDamage;
    }

    protected override IEnumerator Death()
    {
        StopAttacking();

        GetComponent<Collider2D>().enabled = false;
        healthBar.gameObject.SetActive(false);
        nextAttackBar.transform.parent.gameObject.SetActive(false);
       // sprite.gameObject.SetActive(false);

        if (path)
            path.enabled = false;

        if (rb)
            rb.velocity = Vector2.zero;

        playerState = PlayerState.IMMOBILE;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

    protected override void Die()
    {

        numEnemies--;
        if (numEnemies == 0)
            areAllEnemiesDead = true;

        if(player.GetComponent<PlayerWithJoystick>().onEnemySlain != null)
            player.GetComponent<PlayerWithJoystick>().onEnemySlain(enemyName);      // u StandardEnemy?

        base.Die();
    }

    public override Vector2 GetFacingDirection()
    {
        return path.velocity;
    }

}
