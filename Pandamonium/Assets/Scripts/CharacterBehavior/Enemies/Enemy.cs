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

    protected Vector2 startPos;

    protected Room room;

    public override void Start()
    {
        numEnemies ++;

        base.Start();

        healthBar = transform.Find("EnemyCanvas").Find("HealthBarBG").Find("HealthBar").GetComponent<Image>();

        type = CharacterType.ENEMY;

        room = LevelGeneration.I.GetRoomAtPos(transform.position);
        startPos = transform.position;

        approxPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y)) + new Vector2(0.5f, 0.5f);
        currBounds = new Bounds(approxPosition, Vector3.one);

        GraphUpdateObject guo = new GraphUpdateObject(currBounds)
        {
            updatePhysics = true,
            modifyTag = true,
            setTag = (int)type + 1
        };
        AstarPath.active.UpdateGraphs(guo);
        player = GameManager.I.playerInstance.transform;
    }

    protected override void Update()
    {

        base.Update();

        UpdateGraph();

        switch (playerState)
        {

            case PlayerState.ATTACKING:

            case PlayerState.CHASING_ENEMY:

                if(LevelGeneration.I.GetRoomAtPos(target.position) != room)
                {
                    MoveToPosition(startPos);
                }

                break;

            case PlayerState.IDLE:
                {
                    Transform closest;

                    if ((closest = vision.GetClosest()) != null && LevelGeneration.I.GetRoomAtPos(closest.position) == room)
                    {
                        Attack(closest);
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

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);
        healthBar.fillAmount = health / maxHealth;

    }

    public override void Die()
    {
        numEnemies--;
        if (numEnemies == 0)
            areAllEnemiesDead = true;

        GraphUpdateObject guo = new GraphUpdateObject(currBounds)
        {
            updatePhysics = true,
            modifyTag = true,
            setTag = 0
        };
        AstarPath.active.UpdateGraphs(guo);

        base.Die();
    }

}
