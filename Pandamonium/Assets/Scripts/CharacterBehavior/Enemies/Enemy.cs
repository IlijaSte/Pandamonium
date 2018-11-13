using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    protected Transform player;
    private static int numEnemies = 0;
    private BoardCreator boardCreator;

    protected Vector2 startPos;

    protected Room room;

    public override void Start()
    {
        numEnemies ++;

        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        healthBar = transform.Find("EnemyCanvas").Find("HealthBarBG").Find("HealthBar").GetComponent<Image>();

        type = CharacterType.ENEMY;

        boardCreator = BoardCreator.I;
        room = Room.GetRoomAtPos(transform.position);
        startPos = transform.position;
    }

    protected override void Update()
    {

        base.Update();

        UpdateGraph();

        switch (playerState)
        {

            case PlayerState.ATTACKING:

            case PlayerState.CHASING_ENEMY:

                if(Room.GetRoomAtPos(target.position) != room)
                {
                    MoveToPosition(startPos);
                }

                break;

            case PlayerState.IDLE:
                {
                    Transform closest;

                    if ((closest = vision.GetClosest()) != null && Room.GetRoomAtPos(closest.position) == room)
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

                        if ((closest = vision.GetClosest()) != null && Room.GetRoomAtPos(closest.position) == room)
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
        base.Die();
        if (boardCreator.isTutorial && numEnemies == 0)
            boardCreator.InstantiateFinishCollider();
    }

}
