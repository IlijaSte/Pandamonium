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

    public GameObject[] dropPrefabs;

    protected Vector2 startPos;

    protected Room room;

    protected bool detectedPlayer = false;

    public override void Start()
    {
        numEnemies ++;

        base.Start();

        //healthBar = transform.GetComponentsInChildren<ChaosHealtBar>()[0];//.Find("HealthBarBG").Find("HealthBar").GetComponent<ChaosHealtBar>();

        type = CharacterType.ENEMY;

        room = LevelGeneration.I.GetRoomAtPos(transform.position);
        startPos = transform.position;

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

    protected void DropItem()
    {
        if(Random.Range(0, (float)1) >= 0.95f && dropPrefabs.Length > 0)
            Instantiate(dropPrefabs[Random.Range(0, dropPrefabs.Length)], transform.position, Quaternion.identity);
    }

    protected override void Update()
    {

        base.Update();

        UpdateGraph();

        switch (playerState)
        {

            case PlayerState.ATTACKING:

            case PlayerState.CHASING_ENEMY:

                if (target == null)
                {
                    StopAttacking();
                    StopMoving();
                    detectedPlayer = false;
                }
                else
                {

                    if (LevelGeneration.I.GetRoomAtPos(target.position) != room)
                    {
                        MoveToPosition(startPos);
                        detectedPlayer = false;
                    }
                }

                break;

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

            case PlayerState.WALKING:
                {
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

    public override void Die()
    {
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
