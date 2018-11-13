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
        Wall, Floor, Wall2, WallTop, Acid, 
    }


    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.\
    public IntRange numBonusRooms = new IntRange(5, 8);
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange bossRoomWidth = new IntRange(10, 15);
    public IntRange bossRoomHeight = new IntRange(10, 15);
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.

    public int fragmentWidth = -1;
    public int fragmentHeight = -1;

    public IntRange numEnemies = new IntRange(3, 6);

    public TileBase[] floorTiles;                           // An array of floor tile prefabs.

    public TileBase wallLevel1;                            // An array of wall tile prefabs.
    public TileBase wallLevel2;

    public TileBase wallTop;

    public TileBase[] outerWallTiles;                       // An array of outer wall tile prefabs.

    public TileBase acid;

    public GameObject player;

    [HideInInspector]
    public TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    [HideInInspector]
    public Room[] rooms;                                     // All the rooms that are created for this board.
    private Room bossRoom;
    protected Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    public Transform enemyParent;
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;

    public ColliderGenerator generator;

    public float xCoord { get; private set; }
    public float yCoord { get; private set; }

    [HideInInspector]
    public Fragment[][] fragments;
    [HideInInspector]
    public int widthInFragments;
    [HideInInspector]
    public int heightInFragments;

    public static BoardCreator I = null;

    [HideInInspector]
    public Room[] bonusRooms;
    [HideInInspector]
    public Corridor[] bonusCorridors;

    private void InitializeFragments()
    {

        if (fragmentWidth == -1 || fragmentHeight == -1)
        {
            fragmentWidth = roomWidth.m_Max + 2;
            fragmentHeight = roomHeight.m_Max + 2;
        }

        widthInFragments = columns / fragmentWidth;
        heightInFragments = rows / fragmentHeight;

        fragments = new Fragment[heightInFragments][];

        for (int i = 0; i < heightInFragments; i++)
        {

            fragments[i] = new Fragment[widthInFragments];

            for (int j = 0; j < widthInFragments; j++)
            {
                fragments[i][j] = new Fragment(this, j * fragmentWidth, i * fragmentHeight);
            }
        }
    }



    private void Awake()
    {

        // Singleton pattern
        if (I == null)
            I = this;

        else if (I != this)
            Destroy(gameObject);

        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        InitializeFragments();

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

    protected void InstantiatePlayer()
    {
        Vector2 playerPos = rooms[0].GetRandomPos();
        player.transform.position = playerPos;
    }
    
  
    protected void InstantiateEnemies()
    {
        int definiteNumEnemies = numEnemies.Random;
        for(int i = 0; i < definiteNumEnemies; i++)
        {
            int roomIndex = Random.Range(1, rooms.Length - 1);

            Vector3 enemyPos = rooms[roomIndex].GetRandomPos() + new Vector2(0.5f, 0.5f);
            Instantiate(enemyPrefabs[Mathf.RoundToInt(Random.Range(0, enemyPrefabs.Length))], enemyPos, player.transform.rotation, enemyParent);
        }
    }

    protected void InstantiateBoss()
    {
        int roomIndex = rooms.Length - 1;
        Vector3 bossPos = new Vector3(rooms[roomIndex].xPos + rooms[roomIndex].roomWidth / 2, rooms[roomIndex].yPos + 2 * rooms[roomIndex].roomHeight / 3, player.transform.position.z);

        Instantiate(bossPrefab, bossPos, player.transform.rotation, null);
    }



    protected virtual IEnumerator Start()
    {

        int nodeSizeFactor = (int)(1 / AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.SetDimensions(nodeSizeFactor * columns, nodeSizeFactor * rows, AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.center = new Vector3(columns / 2, rows / 2, 0);

        yield return new WaitForEndOfFrame();
        AstarPath.active.Scan();

        InstantiatePlayer();
        InstantiateEnemies();
        InstantiateBoss();
        
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
                    int height;
                    if((height = GetHeight(j, i)) >= 3)
                    {
                        tiles[j][i] = TileType.WallTop;

                    }else if(height == 2)
                    {
                        tiles[j][i] = TileType.Wall2;
                    }
                    else
                    {
                        if(i + 1 < rows && tiles[j][i + 1] == TileType.Floor)
                        {
                            tiles[j][i] = TileType.Wall2;
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

    public void CreateAcid(Room room)
    {
        if (room.roomWidth > 5 && room.roomHeight > 5)
        {
            int acidWidth = new IntRange(2, room.roomWidth - 1).Random;
            int acidHeight = new IntRange(2, room.roomHeight - 1).Random;

            int acidX = new IntRange(room.xPos + 1, room.xPos + room.roomWidth - 1 - acidWidth).Random;
            int acidY = new IntRange(room.yPos + 1, room.yPos + room.roomHeight - 1 - acidHeight).Random;

            for (int i = 0; i < acidHeight; i++)
            {
                for (int j = 0; j < acidWidth; j++)
                {
                    //if(i == acidHeight - 1)
                    //    tiles[acid]
                    tiles[acidX + j][acidY + i] = TileType.Acid;
                }
            }

        }
    }

    bool CreateBonusRooms()
    {

        int currBonus = 0;

        for(int i = 1; i < rooms.Length; i++)
        {

            int numBonus = new IntRange(0, Mathf.Clamp(bonusRooms.Length - currBonus, 0, 3)).Random;
            while (numBonus > 0)
            {
                bonusCorridors[currBonus] = new Corridor(this, rooms[i].fragment);

                IntRange width = roomWidth;//(i == corridors.Length - 1 ? bossRoomWidth : roomWidth);
                IntRange height = roomHeight;// (i == corridors.Length - 1 ? bossRoomHeight : roomHeight);

                if (!bonusCorridors[currBonus].SetupCorridor(rooms[i], corridorLength, width, height, columns, rows))
                {

                    return false;
                }

                // Create a room.
                bonusRooms[currBonus] = new Room(this);

                // Setup the room based on the previous corridor.
                bonusRooms[currBonus].SetupRoom(roomWidth, roomHeight, columns, rows, bonusCorridors[currBonus], true);

                numBonus--;
                currBonus++;
            }

        }

        return true;

    }

    void ReinitializeArrays()
    {
        for (int j = 0; j < rooms.Length; j++)
        {
            if (rooms[j] != null)
                rooms[j].fragment.SetRoom(null);

            rooms[j] = null;

            if (j < corridors.Length)
            {
                corridors[j] = null;
            }
        }

        for (int j = 0; j < bonusRooms.Length; j++)
        {
            if (bonusRooms[j] != null)
                bonusRooms[j].fragment.SetRoom(null);

            bonusRooms[j] = null;

            bonusCorridors[j] = null;
        }
    }

    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        bonusRooms = new Room[numBonusRooms.Random];

        bonusCorridors = new Corridor[bonusRooms.Length];

        bool validLayout;

        do
        {
            validLayout = true;
            // Create the first room and corridor.
            rooms[0] = new Room(this);

            // Setup the first room, there is no previous corridor so we do not use one.
            rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

            corridors[0] = new Corridor(this, rooms[0].fragment);

            

            // Setup the first corridor using the first room.
            corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

            for (int i = 1; i < rooms.Length; i++)
            {

                /*if (i == rooms.Length - 1 && !isTutorial)
                {
                    rooms[i] = bossRoom = new Room(this);
                    bossRoom.SetupRoom(bossRoomWidth, bossRoomHeight, columns, rows, corridors[i - 1]);
                    break;
                }*/

                // Create a room.
                rooms[i] = new Room(this);

                // Setup the room based on the previous corridor.
                rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

                //CreateAcid(rooms[i]);

                // If we haven't reached the end of the corridors array...
                if (i < corridors.Length)
                {
                    // ... create a corridor.
                    corridors[i] = new Corridor(this, rooms[i].fragment);

                    IntRange width = roomWidth;//(i == corridors.Length - 1 ? bossRoomWidth : roomWidth);
                    IntRange height = roomHeight;// (i == corridors.Length - 1 ? bossRoomHeight : roomHeight);

                    if (!corridors[i].SetupCorridor(rooms[i], corridorLength, width, height, columns, rows))
                    {
                        print("PROBLEM!");
                        validLayout = false;

                        ReinitializeArrays();

                        break;
                    }

                }

            }

            if (validLayout)
            {
                /*if (currBonus < bonusRooms.Length - 1)
                {
                    print("BONUS PROBLEM!");
                    ReinitializeArrays();
                    validLayout = false;
                }*/

                if (!CreateBonusRooms())
                {
                    print("BONUS PROBLEM!");
                    ReinitializeArrays();
                    validLayout = false;
                }

            }
        } while (!validLayout);

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
                    if(tiles[xCoord][yCoord] != TileType.Acid)
                        tiles[xCoord][yCoord] = TileType.Floor;  
                   
                }
            }

        }

        // Go through all the rooms...
        for (int i = 0; i < bonusRooms.Length; i++)
        {
            Room currentRoom = bonusRooms[i];

            if (currentRoom == null)
                continue;

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.
                    if (tiles[xCoord][yCoord] != TileType.Acid)
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
            }


        }

        // Go through every corridor...
        for (int i = 0; i < bonusCorridors.Length; i++)
        {
            Corridor currentCorridor = bonusCorridors[i];

            if (currentCorridor == null)
                continue;

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
                if(tiles[i][j] != TileType.Acid)
                    InstantiateFromArray(floorTiles, groundTilemap, i, j);

                switch (tiles[i][j])
                {
                    case TileType.Wall:
                        InstantiateTile(wallLevel1, obstacleTilemap, i, j);
                        break;

                    case TileType.Wall2:
                        InstantiateTile(wallLevel2, obstacleTilemap, i, j);
                        break;

                    case TileType.WallTop:
                        InstantiateTile(wallTop, obstacleTilemap, i, j);
                        break;

                    case TileType.Acid:
                        InstantiateTile(acid, obstacleTilemap, i, j);
                        break;

                }
                    // ... instantiate a wall over the top.
                    
                    
                //}else if(tiles[i][j] == TileT)
            }
        }

        obstacleTilemap.RefreshAllTiles();
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

   

    public void Update()
    {
        for(int i = 0; i < heightInFragments; i++)
        {
            for(int j = 0; j < widthInFragments; j++)
            {

                Debug.DrawLine(new Vector3(fragments[i][j].xPos, fragments[i][j].yPos), new Vector3(fragments[i][j].xPos + fragmentWidth, fragments[i][j].yPos));
                Debug.DrawLine(new Vector3(fragments[i][j].xPos, fragments[i][j].yPos), new Vector3(fragments[i][j].xPos, fragments[i][j].yPos + fragmentHeight));
                Debug.DrawLine(new Vector3(fragments[i][j].xPos, fragments[i][j].yPos + fragmentHeight), new Vector3(fragments[i][j].xPos + fragmentWidth, fragments[i][j].yPos + fragmentHeight));
                Debug.DrawLine(new Vector3(fragments[i][j].xPos + fragmentWidth, fragments[i][j].yPos), new Vector3(fragments[i][j].xPos + fragmentWidth, fragments[i][j].yPos + fragmentHeight));


            }
        }
    }

}