using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class TutorialBoardCreator : BoardCreator {

    public bool isTutorial;
    public GameObject tutorialParentCollider;
    public GameObject tutorialCollider;
    private static Vector2 tutorialFinishPosition;
   // public GameObject enemyParent;

    public override void Update()
    {
        base.Update();
        if (Enemy.areAllEnemiesDead)
            InstantiateFinishCollider();
    }

    protected override IEnumerator Start()
    {
        int nodeSizeFactor = (int)(1 / AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.SetDimensions(nodeSizeFactor * columns, nodeSizeFactor * rows, AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.center = new Vector3(columns / 2, rows / 2, 0);

        yield return new WaitForEndOfFrame();
        AstarPath.active.Scan();

        InstantiatePlayer();

        InstantiateTutorialEnemies();
        InstantiateTutorialColliders();


    }

    protected void InstantiateTutorialEnemies()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                int randomIntX = Random.Range(2, rooms[i].roomWidth);
                int randomIntY = Random.Range(2, rooms[i].roomHeight);
                Vector3 enemyPos = new Vector3(rooms[i].xPos + randomIntX, rooms[i].yPos + randomIntY, player.transform.position.z);
                Instantiate(enemyPrefabs[0], enemyPos, player.transform.rotation, enemyParent);
            }

        }

    }

    private void InstantiateTutorialColliders()
    {
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];
            int xCoord = currentCorridor.startXPos;
            int yCoord = currentCorridor.startYPos;
            int k = currentCorridor.corridorLength - 2;

            switch (currentCorridor.direction)
            {
                case Direction.North:
                    yCoord += k;
                    break;
                case Direction.East:
                    xCoord += k;
                    break;
                case Direction.South:
                    yCoord -= k;
                    break;
                case Direction.West:
                    xCoord -= k;
                    break;
            }

            GameObject newCollider = Instantiate(tutorialCollider, new Vector3(xCoord + 0.5f, yCoord + 0.5f, 0), Quaternion.identity, tutorialParentCollider.transform);
            newCollider.GetComponent<TutorialCollidersScript>().colliderID = i;

        
       

        }

    }

    private void InstantiateFinishCollider()
    {
       Room room = rooms[rooms.Length - 1];
        tutorialFinishPosition.x = room.xPos + room.roomWidth / 2 + 0.5f;
        tutorialFinishPosition.y = room.yPos + room.roomHeight / 2 + 0.5f;

       GameObject newCollider = Instantiate(tutorialCollider, new Vector3(tutorialFinishPosition.x, tutorialFinishPosition.y, 0), Quaternion.identity, tutorialParentCollider.transform);
       newCollider.GetComponent<TutorialCollidersScript>().colliderID = -1;


        /*
        if (isTutorial && i == rooms.Length - 1 && j == (currentRoom.roomWidth / 2) && k == (currentRoom.roomHeight / 2))
        {
            float finishX = xCoord + 0.5f;
            float finishY = yCoord + 0.5f;
            tutorialFinishPosition = new Vector2(finishX, finishY);
         }
        */
    /*}

}*/
