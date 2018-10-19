using System.Collections;

using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Pathfinding;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Wall2, WallAngle, WallAngleBottomRight, WallAngleBottomLeft,
        WallTop, WallAngleLeft, WallAngleRight, WallAngleTop, WallAngleLinkLeft, WallAngleLinkRight,
        WallAngleTopRight, WallAngleTopLeft, WallAngleTopDouble, WallAngleBottomDouble,
        WallAngleLeftDouble, WallAngleRightDouble, WallAngleHorizontalDouble, WallAngleVerticalDouble
    }


    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange bossRoomWidth = new IntRange(10, 15);
    public IntRange bossRoomHeight = new IntRange(10, 15);
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.

    public IntRange numEnemies = new IntRange(3, 6);

    public TileBase[] floorTiles;                           // An array of floor tile prefabs.

    public TileBase wallLevel1;                            // An array of wall tile prefabs.
    public TileBase wallLevel2;
    public TileBase wallAngle;
    public TileBase wallTop;
    public TileBase wallAngleBottomLeft;
    public TileBase wallAngleBottomRight;
    public TileBase wallAngleLeft;
    public TileBase wallAngleRight;
    public TileBase wallAngleTop;
    public TileBase wallAngleLinkLeft;
    public TileBase wallAngleLinkRight;
    public TileBase wallAngleTopLeft;
    public TileBase wallAngleTopRight;
    public TileBase wallAngleTopDouble;
    public TileBase wallAngleBottomDouble;
    public TileBase wallAngleLeftDouble;
    public TileBase wallAngleRightDouble;
    public TileBase wallAngleHorizontalDouble;
    public TileBase wallAngleVerticalDouble;

    public TileBase[] outerWallTiles;                       // An array of outer wall tile prefabs.
    public GameObject player;

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Room bossRoom;
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    public ColliderGenerator generator;

    public bool isTutorial;
    public GameObject tutorialParentCollider;
    public GameObject tutorialCollider;

    private void Awake()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();

        MakeWallHeight();

        InstantiateTiles();
        InstantiateOuterWalls();

        GameObject.FindGameObjectWithTag("Ground").transform.localScale = new Vector3(columns, rows, 1);
        GameObject.FindGameObjectWithTag("Ground").transform.position = new Vector3(columns / 2, rows / 2, 2);
        Camera.main.GetComponent<CameraMovement>().SetBounds(new Vector2(0, 0), new Vector2(columns, rows));

        generator.Generate(columns, rows);

    }

   

    void InstantiatePlayer()
    {
        Vector3 playerPos = new Vector3(rooms[0].xPos + 1, rooms[0].yPos + 1, player.transform.position.z);
        player.transform.position = playerPos;
    }

    void InstantiateTutorialEnemies()
    {
        for(int i = 0; i < rooms.Length; i++)
        {
            for(int j = 0; j < i; j ++)
            {
                int randomIntX = Random.Range(2, rooms[i].roomWidth);
                int randomIntY = Random.Range(2, rooms[i].roomHeight);
                Vector3 enemyPos = new Vector3(rooms[i].xPos + randomIntX, rooms[i].yPos + randomIntY, player.transform.position.z);
                Instantiate(enemyPrefab, enemyPos, player.transform.rotation, GameObject.FindGameObjectWithTag("2DWorld").transform);
            }
            
        }
  
    }
    void InstantiateEnemies()
    {
        int definiteNumEnemies = numEnemies.Random;
        for(int i = 0; i < definiteNumEnemies; i++)
        {
            int roomIndex = Random.Range(0, rooms.Length - 1);
            Vector3 enemyPos = new Vector3(rooms[roomIndex].xPos + Random.Range(2, rooms[roomIndex].roomWidth), rooms[roomIndex].yPos + Random.Range(2, rooms[roomIndex].roomHeight), player.transform.position.z);

            Instantiate(enemyPrefab, enemyPos, player.transform.rotation, null);
        }
    }

    void InstantiateBoss()
    {
        int roomIndex = rooms.Length - 1;
        Vector3 bossPos = new Vector3(rooms[roomIndex].xPos + rooms[roomIndex].roomWidth / 2, rooms[roomIndex].yPos + 2 * rooms[roomIndex].roomHeight / 3, player.transform.position.z);

        Instantiate(bossPrefab, bossPos, player.transform.rotation, null);
    }

    IEnumerator Start()
    {

        int nodeSizeFactor = (int)(1 / AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.SetDimensions(nodeSizeFactor * columns, nodeSizeFactor * rows, AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.center = new Vector3(columns / 2, rows / 2, 0);

        yield return new WaitForEndOfFrame();
        AstarPath.active.Scan();

        InstantiatePlayer();

        if (isTutorial)
            InstantiateTutorialEnemies();
        else
        {
            InstantiateEnemies();
            InstantiateBoss();
        }
    }

    private int GetHeight(int x, int y)
    {
        int i = y;
        int height = 0;

        while(i >= 0 && tiles[x][i] != TileType.Floor)
        {
            i--;
            height++;
        }

        return height;
    }

    private void MakeWallHeight()
    {

        for(int i = 0; i < rows; i++)
        {

            for(int j = 0; j < columns; j++)
            {

                if(tiles[j][i] == TileType.Wall)
                {
                    if(i - 1 >= 0)
                    {
                        if(tiles[j][i - 1] == TileType.Floor)
                        {

                            if(j - 1 >= 0 && tiles[j - 1][i] == TileType.WallAngle)
                            {
                                tiles[j - 1][i] = TileType.WallAngleBottomRight;
                            }

                        }
                        if (tiles[j][i - 1] == TileType.Wall)
                        {

                            tiles[j][i] = TileType.Wall2;

                            if(j - 1 >= 0 && tiles[j - 1][i] == TileType.WallAngle)
                            {
                                tiles[j - 1][i] = TileType.WallAngleBottomRight;
                            }

                        }
                        else if (tiles[j][i - 1] == TileType.Wall2)
                        {

                            if (j + 1 < columns && tiles[j + 1][i] == TileType.Floor)
                            {
                                tiles[j][i] = TileType.WallAngleBottomRight;

                                if (j - 1 >= 0 && tiles[j - 1][i] == TileType.WallAngleRight)
                                {
                                    tiles[j - 1][i] = TileType.WallAngleLinkRight;
                                }

                            }
                            else if(j - 1 >= 0 && (tiles[j - 1][i] == TileType.Floor || tiles[j - 1][i] == TileType.Wall || tiles[j - 1][i] == TileType.Wall2))
                            {
                                tiles[j][i] = TileType.WallAngleBottomLeft;
                            }
                            else
                            {
                                tiles[j][i] = TileType.WallAngle;
                            }

                            if (j - 1 >= 0 && tiles[j - 1][i] == TileType.WallAngleRight)
                            {
                                tiles[j - 1][i] = TileType.WallAngleLinkRight;
                            }
                        }
                        else if (tiles[j][i - 1] == TileType.WallTop || tiles[j][i - 1] == TileType.WallAngle ||
                                 tiles[j][i - 1] == TileType.WallAngleLinkLeft || tiles[j][i - 1] == TileType.WallAngleLinkRight)
                        {
                            tiles[j][i] = TileType.WallTop;

                        }else if(tiles[j][i - 1] == TileType.Floor && i + 1 < rows && tiles[j][i + 1] == TileType.Floor)
                        {
                            tiles[j][i] = TileType.Wall2;

                        }else if(tiles[j][i - 1] == TileType.WallAngleBottomLeft || tiles[j][i - 1] == TileType.WallAngleLeft)
                        {
                            if (j - 1 >= 0 && (tiles[j - 1][i] == TileType.WallAngle || tiles[j - 1][i] == TileType.WallAngleBottomLeft))
                                tiles[j][i] = TileType.WallAngleLinkLeft;

                            else
                                tiles[j][i] = TileType.WallAngleLeft;
                        }
                        else if (tiles[j][i - 1] == TileType.WallAngleBottomRight || tiles[j][i - 1] == TileType.WallAngleRight)
                        {
                            tiles[j][i] = TileType.WallAngleRight;
                        }

                    }

                    if(i + 1 < rows)
                    {

                        if(tiles[j][i + 1] == TileType.Floor && i - 1 >= 0 && tiles[j][i - 1] != TileType.Floor && tiles[j][i - 1] != TileType.Wall)
                        {

                            if (tiles[j][i] == TileType.WallAngle)
                            {
                                tiles[j][i] = TileType.WallAngleHorizontalDouble;
                            }
                            else if(tiles[j][i] == TileType.WallTop || tiles[j][i] == TileType.WallAngleLeft)
                            {
                                tiles[j][i] = TileType.WallAngleTop;
                            }

                            if (j + 1 < columns && GetHeight(j + 1, i) < 3)
                            {
                                if (tiles[j][i] == TileType.WallAngleBottomRight)
                                {
                                    tiles[j][i] = TileType.WallAngleRightDouble;
                                }
                                else
                                {
                                    tiles[j][i] = TileType.WallAngleTopRight;

                                    int k = i - 1;
                                    while (k >= 0 && tiles[j][k] == TileType.WallTop && GetHeight(j + 1, k) < 3)
                                    {
                                        tiles[j][k] = TileType.WallAngleRight;
                                        k--;
                                    }
                                }
                            }

                            if(j - 1 >= 0 && GetHeight(j - 1, i) < 3)
                            {
                                if (tiles[j][i] == TileType.WallAngleTopRight)
                                {
                                    tiles[j][i] = TileType.WallAngleTopDouble;
                                }
                                else if(tiles[j][i] == TileType.WallAngleBottomLeft)
                                {
                                    tiles[j][i] = TileType.WallAngleLeftDouble;
                                }else
                                {
                                    tiles[j][i] = TileType.WallAngleTopLeft;

                                    int k = i - 1;
                                    while(k >= 0 && tiles[j][k] == TileType.WallTop && GetHeight(j - 1, k) < 3)
                                    {
                                        tiles[j][k] = TileType.WallAngleLeft;
                                        k--;
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
        }
    }

    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {

            if(i == rooms.Length - 1 && !isTutorial)
            {
                rooms[i] = bossRoom = new Room();
                bossRoom.SetupRoom(bossRoomWidth, bossRoomHeight, columns, rows, corridors[i - 1]);
                break;
            }

            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }
        }


    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.
                    tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }
    }


    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set the tile at these coordinates to Floor.
                tiles[xCoord][yCoord] = TileType.Floor;

                if (isTutorial && j == currentCorridor.corridorLength - 2)
                {

                    GameObject newCollider = Instantiate(tutorialCollider, new Vector3(xCoord + 0.5f, yCoord + 0.5f, 0), Quaternion.identity, tutorialParentCollider.transform);
                    newCollider.GetComponent<TutorialCollidersScript>().colliderID = i;
                }
                   
         
            }

            
        }
    }


    void InstantiateTiles()
    {


        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // ... and instantiate a floor tile for it.
                InstantiateFromArray(floorTiles, groundTilemap, i, j);

                switch (tiles[i][j])
                {
                    case TileType.Wall:
                        InstantiateTile(wallLevel1, obstacleTilemap, i, j);
                        break;

                    case TileType.Wall2:
                        InstantiateTile(wallLevel2, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngle:
                        InstantiateTile(wallAngle, obstacleTilemap, i, j);
                        break;

                    case TileType.WallTop:
                        InstantiateTile(wallTop, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleBottomRight:
                        InstantiateTile(wallAngleBottomRight, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleBottomLeft:
                        InstantiateTile(wallAngleBottomLeft, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleLeft:
                        InstantiateTile(wallAngleLeft, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleRight:
                        InstantiateTile(wallAngleRight, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleLinkLeft:
                        InstantiateTile(wallAngleLinkLeft, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleLinkRight:
                        InstantiateTile(wallAngleLinkRight, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleLeftDouble:
                        InstantiateTile(wallAngleLeftDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleRightDouble:
                        InstantiateTile(wallAngleRightDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleTopDouble:
                        InstantiateTile(wallAngleTopDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleBottomDouble:
                        InstantiateTile(wallAngleBottomDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleHorizontalDouble:
                        InstantiateTile(wallAngleHorizontalDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleVerticalDouble:
                        InstantiateTile(wallAngleVerticalDouble, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleTopLeft:
                        InstantiateTile(wallAngleTopLeft, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleTopRight:
                        InstantiateTile(wallAngleTopRight, obstacleTilemap, i, j);
                        break;

                    case TileType.WallAngleTop:
                        InstantiateTile(wallAngleTop, obstacleTilemap, i, j);
                        break;
                }
                    // ... instantiate a wall over the top.
                    
                    
                //}else if(tiles[i][j] == TileT)
            }
        }
    }


    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(outerWallTiles, obstacleTilemap, xCoord, currentY);

            currentY++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(outerWallTiles, obstacleTilemap, currentX, yCoord);

            currentX++;
        }
    }

    void InstantiateTile(TileBase tile, Tilemap tilemap, float xCoord, float yCoord)
    {
        Vector3 position = new Vector3(xCoord, yCoord, 0);

        // Create an instance of the prefab from the random index of the array.
        //GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
        tilemap.SetTile(tilemap.WorldToCell(position), tile);
    }

    void InstantiateFromArray(TileBase[] prefabs, Tilemap tilemap, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0);

        // Create an instance of the prefab from the random index of the array.
        //GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
        tilemap.SetTile(tilemap.WorldToCell(position), prefabs[randomIndex]);

        // Set the tile's parent to the board holder.
        //tileInstance.transform.parent = boardHolder.transform;
    }
}