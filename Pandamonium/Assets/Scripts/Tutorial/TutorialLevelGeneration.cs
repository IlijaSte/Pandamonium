using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialLevelGeneration : LevelGeneration
{
    public GameObject tutorialParentCollider;
    public GameObject tutorialCollider;
    private static Vector2 tutorialFinishPosition;

    public Tilemap tutorialTileMap;

    public void Update()
    {
        if (Enemy.areAllEnemiesDead)
            InstantiateFinishCollider();
    }


    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());

        InstantiateTutorialColliders();

       
    }

    private void InstantiateTutorialColliders()
    {
        int id = 0;
        foreach(Vector2 currentRoomPosition in takenPositions)
        {
            Room currentRoom = rooms[Mathf.RoundToInt(currentRoomPosition.x) + gridSizeX, Mathf.RoundToInt(currentRoomPosition.y) + gridSizeY];
            Vector3Int positionEndOfCorridor = currentRoom.getRoomHolder().positionEndOfCorridor;
            float x = positionEndOfCorridor.x;
            float y = positionEndOfCorridor.y;
            GameObject newCollider = Instantiate(tutorialCollider, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, tutorialParentCollider.transform);
            newCollider.GetComponent<TutorialCollidersScript>().colliderID = id++;
        }
    }

    protected override void InstantiateEnemies()
    {
      
    }

    private void InstantiateFinishCollider()
    {
        throw new NotImplementedException();
    }

}
