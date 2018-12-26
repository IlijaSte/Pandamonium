using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_0Generation : LevelGeneration {

    public GameObject lanternChoiceRoom;
    public GameObject endRoom;
    public GameObject gauntletRoom;

    protected override void DrawMap()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue; //skip where there is no room
            }

            switch (room.type)
            {
                case Room.RoomType.END:
                    room.Init(endRoom, roomParent);
                    break;
                case Room.RoomType.START:
                    room.Init(firstRoomPrefab, roomParent);
                    break;
                case Room.RoomType.LANTERN_CHOICE:
                    room.Init(lanternChoiceRoom, roomParent);
                    break;
                case Room.RoomType.GAUNTLET:
                    room.Init(gauntletRoom, roomParent);
                    break;
                default:
                    room.Init(GetRandomPrefab(), roomParent);
                    break;
            }


        }
    }

    protected override void InstantiateEnemies() { }

    protected override void InstantiateHealthPool()
    {
        //base.InstantiateHealthPool();

        Room room = rooms[gridSizeX + 1, gridSizeY - 1];

        Vector2 spawnPos;
        Vector2 checkPos;
        do
        {
            spawnPos = room.GetRandomPos();

            checkPos = new Vector2(Mathf.FloorToInt(spawnPos.x), Mathf.FloorToInt(spawnPos.y));

            if (room.getRoomHolder().leftEdge.Equals(checkPos))
            {

            }

        } while (!room.IsTileWalkable(room.groundTilemap, checkPos) ||
                 !room.IsTileWalkable(room.groundTilemap, checkPos + Vector2.right) ||
                 !IsTileFree(checkPos) ||
                 !IsTileFree(checkPos + Vector2.right) ||
                 ((Vector2)room.getRoomHolder().rightEdge.position + 2 * Vector2.left).Equals(checkPos) ||
                 !room.CanSpawnAtPos(checkPos) ||
                 !room.CanSpawnAtPos(checkPos + Vector2.left) ||
                 !room.CanSpawnAtPos(checkPos + Vector2.right) ||
                 !room.CanSpawnAtPos(checkPos + Vector2.up) ||

                 ((Vector2)room.getRoomHolder().rightEdge.position + 2 * Vector2.left + Vector2.up).Equals(checkPos)
                 );

        GameObject healthPool = Instantiate(healthPoolPrefab, spawnPos, Quaternion.identity);

        healthPool.GetComponentInChildren<HealthPool>().canReactivate = true;

        obstaclePositions.Add(spawnPos);
        obstaclePositions.Add(spawnPos + Vector2.right);
    }

    public IEnumerator SpawnEnemiesInRoom(Room room, GameObject prefab, int number)
    {

        yield return null;

        bool hasFrogocid = false;

        while (number > 0)
        {
            Vector2 spawnPos;
            do
            {
                spawnPos = room.GetRandomPos();
            } while (Vector2.Distance(spawnPos, GameManager.I.playerInstance.transform.position) > 10 || enemyPositions.Contains(spawnPos));

            enemyPositions.Add(spawnPos);

            GameObject newPrefab = null;
            GameObject newEnemy;

            newPrefab = prefab;

            newEnemy = Instantiate(newPrefab, spawnPos, Quaternion.identity, enemyParent);

            enemies.Add(newEnemy);
            room.PutEnemy(newEnemy);

            if (!hasFrogocid && newPrefab.GetComponent<AttackingCharacter>() is Frogocite)
            {
                hasFrogocid = true;
            }

            number--;
        }
    }

    protected override void CreateRooms()
    {
        //setup

        rooms = new Room[gridSizeX * 2, gridSizeY * 2];

        rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero, Room.RoomType.START);
        takenPositions.Insert(0, Vector2.zero);

        rooms[gridSizeX, gridSizeY - 1] = new Room(Vector2Int.zero + Vector2Int.down, Room.RoomType.LANTERN_CHOICE);
        takenPositions.Insert(0, Vector2Int.zero + Vector2Int.down);

        rooms[gridSizeX - 1, gridSizeY - 1] = new Room(Vector2Int.zero + Vector2Int.down + Vector2Int.left, Room.RoomType.END);
        takenPositions.Insert(0, Vector2Int.zero + Vector2Int.down + Vector2Int.left);

        rooms[gridSizeX + 1, gridSizeY - 1] = new Room(Vector2Int.zero + Vector2Int.down + Vector2Int.right, Room.RoomType.GAUNTLET);
        takenPositions.Insert(0, Vector2Int.zero + Vector2Int.down + Vector2Int.right);
        
    }

}
