using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour
{
    [Header("General")]
    public Vector2 worldSize = new Vector2(4, 4);

    protected Room[,] rooms;
    [HideInInspector]
    public List<Vector2> takenPositions = new List<Vector2>();

    [HideInInspector]
    public int gridSizeX, gridSizeY;
    protected int numberOfRooms = 20;

    public IntRange roomNumberRange = new IntRange(6, 8);

    public int roomWidth;
    public int roomHeight;

    [Header("Enemies")]

    public float enemyCountMultiplier = 1f;
    public GameObject[] enemyPrefabs;

    public GameObject[] elitePool;

    public GameObject bossPrefab;

    [Header("Rooms")]

    public GameObject[] roomPrefabs;

    public GameObject firstRoomPrefab;
    public GameObject lastRoomPrefab;

    public GameObject bossRoomPrefab;
    public Vector2 bossRoomSize = new Vector2(30, 30);

    public GameObject keyHolderRoom;

    [Header("Prefabs")]

    public GameObject healthPoolPrefab;

    public TileBase corridorHorizPrefab;
    public TileBase corridorVertPrefab;
    public TileBase corridorBridgeHorizPrefab;
    public TileBase corridorBridgeVertPrefab;
    public TileBase corridorHorizForegroundPrefab;
    public TileBase corridorHorizForegroundLeftPrefab;
    public TileBase corridorHorizForegroundRightPrefab;
    public GameObject stairsPrefab;
    public GameObject stairsVertPrefab;

    public TileBase groundPrefab;
    public TileBase acidPrefab;

    [Header("Other")]

    public Tilemap corridorTilemap;

    public Tilemap acidTilemap;

    public GameObject chestPrefab;

    protected Transform roomParent;

    public Transform ground;

    
    protected ArrayList enemies;

    private static LevelGeneration instance;
    protected Transform enemyParent;

    protected ArrayList enemyPositions = new ArrayList();

    protected ArrayList obstaclePositions;

    [HideInInspector]
    public Vector2 bossRoomSpawn;

    protected Room bossRoom;

    public static LevelGeneration I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelGeneration>();
            }
            return instance;
        }
    }

    void Awake()
    {
        numberOfRooms = roomNumberRange.Random;
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        { // make sure we dont try to make more rooms than can fit in our grid
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }

        gridSizeX = Mathf.RoundToInt(worldSize.x); //note: these are half-extents
        gridSizeY = Mathf.RoundToInt(worldSize.y);

        roomParent = new GameObject("Rooms").transform;
        enemyParent = new GameObject("Enemies").transform;
        enemies = new ArrayList();
        Random.InitState(System.Environment.TickCount);


        CreateRooms(); //lays out the actual map
        FillAcid();     // fills room with acid (or other obstacle type in the future)
        SetRoomDoors(); //assigns the doors where rooms would connect
        DrawMap(); //instantiates objects to make up a map
        SetRoomDifficulties();
    }

    protected virtual IEnumerator Start()
    {

        PositionPlayer();

        yield return new WaitForSeconds(0.1f);

        float nodeSize = AstarPath.active.data.gridGraph.nodeSize;
        AstarPath.active.data.gridGraph.SetDimensions(Mathf.RoundToInt((gridSizeX * roomWidth * 2 + roomWidth) / nodeSize), Mathf.RoundToInt((gridSizeY * roomHeight * 2 + roomHeight) / nodeSize), nodeSize);
        ground.localScale = new Vector2(gridSizeX * roomWidth * 2 + roomWidth / 2, gridSizeY * roomHeight * 2 + roomHeight / 2);

        Camera.main.GetComponent<CameraMovement>().SetBounds(new Vector2(-gridSizeX * roomWidth - roomWidth / 2, -gridSizeY * roomHeight - roomHeight / 2), new Vector2(gridSizeX * roomWidth + roomWidth / 2, gridSizeY * roomHeight + roomHeight / 2));

        if (GameManager.I.IsBossLevel())         // boss soba
        {
            (AstarPath.active.graphs[1] as GridGraph).center = (Vector2)BossRoomPosition();
            (AstarPath.active.graphs[1] as GridGraph).SetDimensions(Mathf.RoundToInt(bossRoomSize.x / nodeSize), Mathf.RoundToInt(bossRoomSize.y / nodeSize), nodeSize);

        }

        AstarPath.active.Scan();

        obstaclePositions = new ArrayList();

        InstantiateEnemies();

        InstantiateBoss();

        InstantiateHealthPool();
        if (GameManager.I.currentLevel >= 2)
        {
            InstantiateHealthPool();
        }
    }

    protected void PositionPlayer()
    {
        GameManager.I.playerInstance.transform.position = rooms[gridSizeX, gridSizeY].spawnPoint;
    }

    public void BoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;
        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }

    public void FillAcid()
    {
        BoxFill(acidTilemap, acidPrefab, new Vector3Int(-gridSizeX * roomWidth - roomWidth / 2, -gridSizeY * roomHeight - roomHeight / 2, 0), new Vector3Int(gridSizeX * roomWidth + roomWidth / 2, gridSizeY * roomHeight + roomHeight / 2, 0));

        if (GameManager.I.IsBossLevel())
        {
            BoxFill(acidTilemap, acidPrefab, new Vector3Int(BossRoomPosition().x - roomWidth, BossRoomPosition().y - roomHeight, 0), new Vector3Int(BossRoomPosition().x + roomWidth, BossRoomPosition().y + roomHeight, 0));
        }
    }

    public Room GetRoomAtPos(Vector2 pos)
    {
        if (gridSizeX + Mathf.RoundToInt(pos.x / roomWidth) > gridSizeX * 2 || gridSizeY + Mathf.RoundToInt(pos.y / roomHeight) > gridSizeY * 2)
            return null;

        return rooms[gridSizeX + Mathf.RoundToInt(pos.x / roomWidth), gridSizeY + Mathf.RoundToInt(pos.y / roomHeight)];
    }

    public Room GetRoomAtGridPos(Vector2 gridPos)
    {
        return rooms[gridSizeX + Mathf.RoundToInt(gridPos.x), gridSizeY + Mathf.RoundToInt(gridPos.y)];
    }

    public bool IsTileFree(Vector2 pos)
    {
        pos += new Vector2(0.45f, 0.45f);
        Vector3Int tilePos = acidTilemap.WorldToCell(pos);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.transform != null && acidTilemap.WorldToCell(enemy.transform.position).Equals(tilePos))
            {
                return false;
            }
        }

        foreach(Vector2 obstacle in obstaclePositions)
        {
            if(obstacle != null && acidTilemap.WorldToCell(obstacle).Equals(tilePos))
            {
                return false;
            }
        }

        return true;
    }

    protected virtual void SetRoomDifficulties()
    {
        Stack<Room> roomsToTraverse = new Stack<Room>();
        ArrayList passedRooms = new ArrayList();

        rooms[gridSizeX, gridSizeY].distanceFromStart = 0;
        roomsToTraverse.Push(rooms[gridSizeX, gridSizeY]);

        while (roomsToTraverse.Count > 0)
        {

            Room currRoom = roomsToTraverse.Pop();

            if (gridSizeX + currRoom.gridPos.x + 1 < gridSizeX * 2)
            {
                Room room = rooms[gridSizeX + currRoom.gridPos.x + 1, gridSizeY + currRoom.gridPos.y];
                if (room != null && !passedRooms.Contains(room) && currRoom.doorRight)
                {

                    roomsToTraverse.Push(room);
                    room.distanceFromStart = currRoom.distanceFromStart + 1;
                }
            }
            if (gridSizeX + currRoom.gridPos.x - 1 >= 0)
            {
                Room room = rooms[gridSizeX + currRoom.gridPos.x - 1, gridSizeY + currRoom.gridPos.y];
                if (room != null && !passedRooms.Contains(room) && currRoom.doorLeft)
                {
                    roomsToTraverse.Push(room);
                    room.distanceFromStart = currRoom.distanceFromStart + 1;
                }
            }
            if (gridSizeY + currRoom.gridPos.y + 1 < gridSizeY * 2)
            {
                Room room = rooms[gridSizeX + currRoom.gridPos.x, gridSizeY + currRoom.gridPos.y + 1];
                if (room != null && !passedRooms.Contains(room) && currRoom.doorTop)
                {
                    roomsToTraverse.Push(room);
                    room.distanceFromStart = currRoom.distanceFromStart + 1;
                }
            }
            if (gridSizeY + currRoom.gridPos.y - 1 >= 0)
            {
                Room room = rooms[gridSizeX + currRoom.gridPos.x, gridSizeY + currRoom.gridPos.y - 1];
                if (room != null && !passedRooms.Contains(room) && currRoom.doorBot)
                {
                    roomsToTraverse.Push(room);
                    room.distanceFromStart = currRoom.distanceFromStart + 1;
                }
            }

            passedRooms.Add(currRoom);

        }
    }

    protected virtual void InstantiateHealthPool()
    {
        Vector2 pos;
        Room room;
        do
        {
            pos = takenPositions[Random.Range(1, takenPositions.Count - 2)];
            room = rooms[Mathf.RoundToInt(gridSizeX + pos.x), Mathf.RoundToInt(gridSizeY + pos.y)];
        } while (room.type == Room.RoomType.START || room.type == Room.RoomType.OBELISK || room.type == Room.RoomType.KEY_HOLDER);

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

        Instantiate(healthPoolPrefab, spawnPos, Quaternion.identity);

        obstaclePositions.Add(spawnPos);
        obstaclePositions.Add(spawnPos + Vector2.right);
    }

    public virtual void InstantiateEnemiesInRoom(Room room)
    {
        // broj neprijatelja u sobi zavisi od razdaljine sobe od pocetne sobe i multiplier-a

        float levelDifficulty = 0;
        if (GameManager.I.currentLevel > 2)
            levelDifficulty = (GameManager.I.currentLevel - 2) * 0.5f;

        int totalDifficulty = Mathf.RoundToInt(Random.Range(room.distanceFromStart * enemyCountMultiplier + levelDifficulty, room.distanceFromStart * enemyCountMultiplier + levelDifficulty + 1.5f));

        bool hasFrogocid = false;

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

            // da bi se u prve dve sobe stvarao samo prvi tip protivnika - verovatno menjati za sledece levele


            do
            {
                newPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            } while (((newPrefab.GetComponent<AttackingCharacter>() is RangedFrogocite) && !hasFrogocid) || newPrefab.GetComponent<Enemy>().difficulty > totalDifficulty);

            newEnemy = Instantiate(newPrefab, spawnPos, Quaternion.identity, enemyParent);

            enemies.Add(newEnemy);
            room.PutEnemy(newEnemy);

            if (newPrefab.GetComponent<AttackingCharacter>() is Frogocite)
            {
                hasFrogocid = true;
            }

            totalDifficulty -= newEnemy.GetComponent<Enemy>().difficulty;
        }
    }

    protected virtual void InstantiateEnemies()
    {

        foreach (Vector2 pos in takenPositions)
        {
            Room room = rooms[Mathf.RoundToInt(gridSizeX + pos.x), Mathf.RoundToInt(gridSizeY + pos.y)];

            if (room.type == Room.RoomType.START || room.type == Room.RoomType.OBELISK || room.type == Room.RoomType.KEY_HOLDER)
                continue;

            // elite

            if (room.type == Room.RoomType.ELITE)
            {
                //Vector2 spawnPos = room.GetSpawnPoint();
                Vector2 spawnPos = room.getRoomHolder().transform.position;

                GameObject newEnemy = Instantiate(elitePool[Random.Range(0, elitePool.Length)], spawnPos, Quaternion.identity, enemyParent);

                //enemies.Add(newEnemy);            // !!!
                room.PutEnemy(newEnemy);
                continue;
            }

            InstantiateEnemiesInRoom(room);

        }

        (enemies[Random.Range(0, enemies.Count)] as GameObject).GetComponent<Enemy>().holdsKey = true;
    }

    protected virtual void InstantiateBoss()
    {
        if (GameManager.I.IsBossLevel())
        {
            //Vector2 bossSpawnPos = bossRoom.getRoomHolder().transform.position;
            //Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity, enemyParent);
        }
    }

    protected virtual int CreateIntroRooms()
    {
        rooms[gridSizeX, gridSizeY - 1] = new Room(new Vector2Int(0, -1), Room.RoomType.DEFAULT);
        takenPositions.Insert(0, new Vector2Int(0, -1));
        return 1;
    }

    protected virtual void CreateRooms()
    {
        //setup
        int obeliskIterations = 0;

        Vector2Int obeliskCheckPos;

        bool spawnKeyHolder = GameManager.I.currentLevel > 0;

        do
        {
            takenPositions.Clear();
            rooms = new Room[gridSizeX * 2, gridSizeY * 2];
            rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero, Room.RoomType.START);
            takenPositions.Insert(0, Vector2.zero);

            int introRooms = CreateIntroRooms();

            int keyHolderIndex = Random.Range(0, numberOfRooms - introRooms);

            Vector2Int checkPos = Vector2Int.zero;

            Room nextTo;

            //magic numbers
            float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
            //add rooms
            for (int i = 0; i < numberOfRooms - introRooms; i++)
            {

                float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
                randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
                //grab new position
                checkPos = NewPosition(out nextTo);
                //test new position
                if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
                {
                    int iterations = 0;
                    do
                    {

                        checkPos = SelectiveNewPosition(out nextTo);
                        iterations++;
                    } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
                    if (iterations >= 50)
                        print("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
                }
                //finalize position

                if(spawnKeyHolder && i == keyHolderIndex)
                {
                    rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.KEY_HOLDER);
                }
                else
                {
                    rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.DEFAULT);
                }
                
                takenPositions.Insert(0, checkPos);
            }

            bool valid;

            // elite soba

            Room eliteRoom = null;

            if(GameManager.I.currentLevel > 0 && ((GameManager.I.currentLevel + 1) % 3) != 0)
            {
                Vector2Int elitePos;

                do {
                    elitePos = NewPosition(out nextTo);
                } while (elitePos.y + 1 >= gridSizeY || rooms[gridSizeX + elitePos.x, gridSizeY + elitePos.y + 1] != null);

                eliteRoom = rooms[elitePos.x + gridSizeX, elitePos.y + gridSizeY] = new Room(elitePos, Room.RoomType.ELITE);
                takenPositions.Insert(0, elitePos);
                eliteRoom.nextTo = nextTo;
            }

            // obelisk soba

            valid = ObeliskPosition(out obeliskCheckPos, eliteRoom);

            if (NumberOfNeighbors(obeliskCheckPos, takenPositions) > 3)
            {
                obeliskIterations = 0;
                do
                {
                    valid = ObeliskPosition(out obeliskCheckPos, eliteRoom);
                    obeliskIterations++;
                } while (!valid && obeliskIterations < 100);
                if (obeliskIterations >= 50)
                    print("error: could not create obelisk room with fewer neighbors than : " + NumberOfNeighbors(obeliskCheckPos, takenPositions));
            }

        } while (obeliskIterations >= 100);


        rooms[(int)obeliskCheckPos.x + gridSizeX, (int)obeliskCheckPos.y + gridSizeY] = new Room(obeliskCheckPos, Room.RoomType.OBELISK);
        takenPositions.Insert(0, obeliskCheckPos);

        // boss soba
        if(GameManager.I.IsBossLevel())
        {
            Vector2Int bossRoomPos = new Vector2Int(Mathf.RoundToInt(BossRoomPosition().x / (float)roomWidth), Mathf.RoundToInt(BossRoomPosition().y / (float)roomHeight));
            bossRoom = new Room(bossRoomPos, Room.RoomType.BOSS);
        }

    }

    protected Vector2Int BossRoomPosition()
    {
        return new Vector2Int(gridSizeX * roomWidth * 4, 0);
    }

    protected Vector2Int NewPosition(out Room nextTo)
    {
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        Vector2 neighborPos = Vector2.zero;
        Room neighborRoom;

        do
        {
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // pick a random room
            x = (int)takenPositions[index].x;//capture its x, y position
            y = (int)takenPositions[index].y;

            neighborPos = takenPositions[index];
            neighborRoom = GetRoomAtGridPos(neighborPos);

            bool UpDown = (Random.value < 0.5f);//randomly pick wether to look on hor or vert axis
            bool positive = (Random.value < 0.5f);//pick whether to be positive or negative on that axis
            if (UpDown)
            { //find the position bnased on the above bools
                if (positive)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (positive)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }
            checkingPos = new Vector2Int(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY || (takenPositions.Count > 1 && neighborPos.Equals(Vector2.zero)) || (takenPositions.Count > 2 && neighborRoom.type == Room.RoomType.INTRO)); //make sure the position is valid

        nextTo = neighborRoom;

        return checkingPos;
    }

    bool ObeliskPosition(out Vector2Int pos, Room enteringFrom = null)
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        Vector2 neighborPos = Vector2.zero;
        Room neighborRoom;

        int i = 0;

        do
        {
            inc = 0;

            if (enteringFrom == null)
            {

                do
                {
                    //instead of getting a room to find an adject empty space, we start with one that only 
                    //as one neighbor. This will make it more likely that it returns a room that branches out
                    index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                    inc++;
                } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 3 && inc < 100);
                x = (int)takenPositions[index].x;
                y = (int)takenPositions[index].y;
                neighborPos = takenPositions[index];
                neighborRoom = GetRoomAtGridPos(neighborPos);
            }
            else
            {
                x = enteringFrom.gridPos.x;
                y = enteringFrom.gridPos.y;
                neighborPos = enteringFrom.gridPos;
                neighborRoom = enteringFrom;
            }

            y += 1;

            checkingPos = new Vector2Int(x, y);

            i++;
        } while (i < 100 && (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY || neighborPos.Equals(Vector2.zero)) || (takenPositions.Count > 2 && neighborRoom.type == Room.RoomType.INTRO));
        if (inc >= 100)
        { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
            print("Error: could not find position with only one neighbor");
        }

        if (i > 100)
        {
            print("Error: could not find a suitable position for the obelisk room");
            pos = Vector2Int.zero;
            return false;
        }
        pos = checkingPos;
        return true;
    }

    Vector2Int SelectiveNewPosition(out Room nextTo)
    { // method differs from the above in the two commented ways
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        Vector2 neighborPos = Vector2.zero;
        Room neighborRoom;
        do
        {
            inc = 0;
            do
            {
                //instead of getting a room to find an adject empty space, we start with one that only 
                //as one neighbor. This will make it more likely that it returns a room that branches out
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            neighborPos = takenPositions[index];
            neighborRoom = GetRoomAtGridPos(neighborPos);

            bool UpDown = (Random.value < 0.5f);
            bool positive = (Random.value < 0.5f);
            if (UpDown)
            {
                if (positive)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (positive)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }
            checkingPos = new Vector2Int(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY || (takenPositions.Count > 1 && neighborPos.Equals(Vector2.zero)) || (takenPositions.Count > 2 && neighborRoom.type == Room.RoomType.INTRO));
        if (inc >= 100)
        { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
            print("Error: could not find position with only one neighbor");
        }

        nextTo = neighborRoom;

        return checkingPos;
    }

    int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions)
    {
        int ret = 0; // start at zero, add 1 for each side there is already a room
        if (usedPositions.Contains(checkingPos + Vector2.right))
        { //using Vector.[direction] as short hands, for simplicity
            ret++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.left))
        {
            ret++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.up))
        {
            ret++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.down))
        {
            ret++;
        }
        return ret;
    }

    protected virtual void DrawMap()
    {

        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue; //skip where there is no room
            }

            switch (room.type)
            {
                case Room.RoomType.OBELISK:
                    room.Init(lastRoomPrefab, roomParent);
                    break;
                case Room.RoomType.START:
                    room.Init(firstRoomPrefab, roomParent);
                    break;
                case Room.RoomType.KEY_HOLDER:
                    room.Init(keyHolderRoom, roomParent);
                    break;
                default:
                    room.Init(GetRandomPrefab(), roomParent);
                    break;
            }


        }

        if (GameManager.I.IsBossLevel())
        {
            bossRoom.Init(bossRoomPrefab, roomParent);
            bossRoomSpawn = bossRoom.GetSpawnPoint();
        }
    }

    protected GameObject GetRandomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

    void SetRoomDoors()
    {
        for (int x = 0; x < ((gridSizeX * 2)); x++)
        {
            for (int y = 0; y < ((gridSizeY * 2)); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }

                if (rooms[x, y].type == Room.RoomType.OBELISK)
                {
                    rooms[x, y].doorBot = true;
                    rooms[x, y].doorTop = false;
                    rooms[x, y].doorLeft = false;
                    rooms[x, y].doorRight = false;

                    continue;
                }

                if (y - 1 < 0)
                { //check above
                    rooms[x, y].doorBot = false;
                }
                else
                {
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null && (rooms[x, y - 1].type != Room.RoomType.START && rooms[x, y - 1].type != Room.RoomType.OBELISK));

                    if (rooms[x, y - 1] != null && (rooms[x, y - 1].type == Room.RoomType.INTRO || rooms[x, y - 1].type == Room.RoomType.ELITE))
                            rooms[x, y].doorBot = rooms[x, y - 1].nextTo == rooms[x, y];


                }
                if (y + 1 >= gridSizeY * 2)
                { //check bellow
                    rooms[x, y].doorTop = false;
                }
                else
                {
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null && (rooms[x, y + 1].type != Room.RoomType.START || rooms[x, y + 1].type == Room.RoomType.OBELISK));

                    if (rooms[x, y + 1] != null && (rooms[x, y + 1].type == Room.RoomType.INTRO || rooms[x, y + 1].type == Room.RoomType.ELITE))
                        rooms[x, y].doorTop = rooms[x, y + 1].nextTo == rooms[x, y];
                }
                if (x - 1 < 0)
                { //check left
                    rooms[x, y].doorLeft = false;

                }
                else
                {
                    rooms[x, y].doorLeft = (rooms[x - 1, y] != null && (rooms[x - 1, y].type != Room.RoomType.START && rooms[x - 1, y].type != Room.RoomType.OBELISK));

                    if (rooms[x - 1, y] != null && (rooms[x - 1, y].type == Room.RoomType.INTRO || rooms[x - 1, y].type == Room.RoomType.ELITE))
                        rooms[x, y].doorLeft = rooms[x - 1, y].nextTo == rooms[x, y];

                }
                if (x + 1 >= gridSizeX * 2)
                { //check right
                    rooms[x, y].doorRight = false;

                }
                else
                {
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null && (rooms[x + 1, y].type != Room.RoomType.START && (rooms[x + 1, y].type != Room.RoomType.OBELISK)));

                    if (rooms[x + 1, y] != null && (rooms[x + 1, y].type == Room.RoomType.INTRO || rooms[x + 1, y].type == Room.RoomType.ELITE))
                        rooms[x, y].doorRight = rooms[x + 1, y].nextTo == rooms[x, y];
                }

                if (rooms[x, y].type == Room.RoomType.INTRO || rooms[x, y].type == Room.RoomType.ELITE)
                {
                    rooms[x, y].doorBot = false;
                    rooms[x, y].doorTop = false;
                    rooms[x, y].doorLeft = false;
                    rooms[x, y].doorRight = false;

                    rooms[x, y].doorBot = rooms[x, y].gridPos.y - rooms[x, y].nextTo.gridPos.y > 0;
                    rooms[x, y].doorTop = (rooms[x, y].gridPos.y - rooms[x, y].nextTo.gridPos.y < 0);

                    rooms[x, y].doorLeft = rooms[x, y].gridPos.x - rooms[x, y].nextTo.gridPos.x > 0;
                    rooms[x, y].doorRight = rooms[x, y].gridPos.x - rooms[x, y].nextTo.gridPos.x < 0;
                }

                if(rooms[x, y].type == Room.RoomType.ELITE)
                {
                    rooms[x, y].doorTop = true;
                }

            }
        }

        // za prvu sobu
        Vector2 secondPos = takenPositions[takenPositions.Count - 2];
        Room secondRoom = rooms[Mathf.RoundToInt(secondPos.x) + gridSizeX, Mathf.RoundToInt(secondPos.y) + gridSizeY];

        Room firstRoom = rooms[gridSizeX, gridSizeY];

        firstRoom.doorBot = 0 - secondPos.y > 0;
        firstRoom.doorTop = 0 - secondPos.y < 0;

        firstRoom.doorLeft = 0 - secondPos.x > 0;
        firstRoom.doorRight = 0 - secondPos.x < 0;

        secondRoom.doorBot = secondRoom.doorBot || firstRoom.doorTop;
        secondRoom.doorTop = secondRoom.doorTop || firstRoom.doorBot;

        secondRoom.doorLeft = secondRoom.doorLeft || firstRoom.doorRight;
        secondRoom.doorRight = secondRoom.doorRight || firstRoom.doorLeft;

    }

    public int GetNumberOfEnemies(string name)
    {

        int num = 0;

        foreach(GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                if (enemy.GetComponent<Enemy>().enemyName.Equals(name))
                {
                    num++;
                }
            }
        }

        return num;
    }
}