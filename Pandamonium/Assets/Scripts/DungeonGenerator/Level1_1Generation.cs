using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_1Generation : LevelGeneration {

    protected override int CreateIntroRooms()
    {

        Room newIntroRoom = rooms[gridSizeX, gridSizeY - 1] = new Room(new Vector2Int(0, -1), Room.RoomType.INTRO);
        takenPositions.Insert(0, new Vector2Int(0, -1));

        Vector2Int checkPos = NewPosition();

        rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.DEFAULT);
        takenPositions.Insert(0, checkPos);

        newIntroRoom.nextTo = rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY];

        return 2;
    }

    protected override void InstantiateEnemies()
    {
        foreach (Vector2 pos in takenPositions)
        {
            Room room = rooms[Mathf.RoundToInt(gridSizeX + pos.x), Mathf.RoundToInt(gridSizeY + pos.y)];

            if (room.type == Room.RoomType.START || room.type == Room.RoomType.OBELISK || room.type == Room.RoomType.KEY_HOLDER)
                continue;

            int totalDifficulty;

            switch (room.distanceFromStart){

                case 1:
                    totalDifficulty = enemyPrefabs[0].GetComponent<Enemy>().difficulty * 2;     // 2 slime-a u prvoj sobi
                    break;
                case 2:
                    totalDifficulty = enemyPrefabs[1].GetComponent<Enemy>().difficulty * 2;     // 2 worm-a u drugoj sobi
                    break;
                default:
                    totalDifficulty = Mathf.RoundToInt(Random.Range(room.distanceFromStart * enemyCountMultiplier, room.distanceFromStart * enemyCountMultiplier + 1.5f));  // broj neprijatelja u sobi zavisi od razdaljine sobe od pocetne sobe i multiplier-a
                    break;

            }

            while (totalDifficulty > 0)
            {
                Vector2 spawnPos;
                do
                {
                    spawnPos = room.GetRandomPos();
                } while (enemyPositions.Contains(spawnPos));

                enemyPositions.Add(spawnPos);

                GameObject newPrefab = null;
                GameObject newEnemy;

                int prefabIndex = 0;

                // da bi se u prve dve sobe stvarao samo prvi tip protivnika - verovatno menjati za sledece levele
                if (room.distanceFromStart == 1)
                {
                    newPrefab = enemyPrefabs[0];
                    prefabIndex = 0;

                }else if(room.distanceFromStart == 2)
                {
                    newPrefab = enemyPrefabs[1];
                    prefabIndex = 1;
                }
                else
                {
                    do
                    {
                        newPrefab = enemyPrefabs[prefabIndex = Random.Range(0, enemyPrefabs.Length)];

                    } while (newPrefab.GetComponent<Enemy>().difficulty > totalDifficulty);
                }

                newEnemy = Instantiate(newPrefab, spawnPos, Quaternion.identity, enemyParent);

                enemies.Add(newEnemy);
                room.PutEnemy(newEnemy);

                totalDifficulty -= newEnemy.GetComponent<Enemy>().difficulty;
            }
        }

        (enemies[Random.Range(0, enemies.Count)] as GameObject).GetComponent<Enemy>().holdsKey = true;
    }

}
