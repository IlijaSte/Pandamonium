using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour {
	public Vector2 worldSize = new Vector2(4,4);
	protected Room[,] rooms;
	protected List<Vector2> takenPositions = new List<Vector2>();
	protected int gridSizeX, gridSizeY;
    public int numberOfRooms = 20;

    public int roomWidth;
    public int roomHeight;

    public float enemyCountMultiplier = 1f;

    public GameObject[] roomPrefabs;

    public GameObject lastRoom;

    public GameObject healthPoolPrefab;

    public TileBase corridorHorizPrefab;
    public TileBase corridorVertPrefab;
    public TileBase corridorBridgeHorizPrefab;
    public TileBase corridorBridgeVertPrefab;

    public TileBase groundPrefab;
    public Tilemap corridorTilemap;

    public Tilemap acidTilemap; 
    public TileBase acidPrefab;

    private Transform roomParent;

    public Transform ground;

    public GameObject[] enemyPrefabs;
    protected ArrayList enemies;

    private static LevelGeneration instance;
    private Transform enemyParent;

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

    void Awake () {

		if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)){ // make sure we dont try to make more rooms than can fit in our grid
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

	}

    protected virtual IEnumerator Start()
    {

        PositionPlayer();

        yield return new WaitForSeconds(0.1f);

        float nodeSize = AstarPath.active.data.gridGraph.nodeSize;
        AstarPath.active.data.gridGraph.SetDimensions(Mathf.RoundToInt((gridSizeX * roomWidth * 2 + roomWidth) / nodeSize), Mathf.RoundToInt((gridSizeY * roomHeight * 2 + roomHeight) / nodeSize), nodeSize);
        ground.localScale = new Vector2(gridSizeX * roomWidth * 2 + roomWidth / 2, gridSizeY * roomHeight * 2 + roomHeight / 2);

        Camera.main.GetComponent<CameraMovement>().SetBounds(new Vector2(-gridSizeX * roomWidth - roomWidth / 2, -gridSizeY * roomHeight - roomHeight / 2), new Vector2(gridSizeX * roomWidth + roomWidth / 2, gridSizeY * roomHeight + roomHeight / 2));

        AstarPath.active.Scan();
        InstantiateEnemies();
        InstantiateHealthPool();
    }

    protected void PositionPlayer()
    {
        GameManager.I.playerInstance.transform.position = rooms[gridSizeX, gridSizeY].GetRandomPos();
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
    }

    public Room GetRoomAtPos(Vector2 pos)
    {
        return rooms[gridSizeX + Mathf.RoundToInt(pos.x / roomWidth), gridSizeY + Mathf.RoundToInt(pos.y / roomHeight)];
    }

    public bool IsTileFree(Vector2 pos)
    {
        Vector3Int tilePos = acidTilemap.WorldToCell(pos);

        foreach(GameObject enemy in enemies)
        {
            if (enemy != null && enemy.transform != null && acidTilemap.WorldToCell(enemy.transform.position).Equals(tilePos))
            {
                return false;
            }
        }

        return true;
    }

    protected virtual void InstantiateHealthPool()
    {
        Vector2 pos = takenPositions[Random.Range(1, takenPositions.Count - 2)];

        Room room = rooms[Mathf.RoundToInt(gridSizeX + pos.x), Mathf.RoundToInt(gridSizeY + pos.y)];

        Vector2 spawnPos;

        do
        {
            spawnPos = room.GetRandomPos();

        } while (!IsTileFree(spawnPos));

        Instantiate(healthPoolPrefab, spawnPos, Quaternion.identity);
    }

    protected virtual void InstantiateEnemies()
    {

        foreach(Vector2 pos in takenPositions)
        {
            Room room = rooms[Mathf.RoundToInt(gridSizeX + pos.x), Mathf.RoundToInt(gridSizeY + pos.y)];

            if (room.type == Room.RoomType.START || room.type == Room.RoomType.OBELISK)
                continue;

            // broj neprijatelja u sobi zavisi od razdaljine sobe od pocetne sobe i multiplier-a
            int totalDifficulty = Mathf.RoundToInt(Random.Range(room.distanceFromStart * enemyCountMultiplier, room.distanceFromStart * enemyCountMultiplier + 1.5f));
            List<Vector2> taken = new List<Vector2>();

            while(totalDifficulty > 0)
            {
                Vector2 spawnPos;
                do
                {
                    spawnPos = room.GetRandomPos();
                } while (taken.Contains(spawnPos));

                taken.Add(spawnPos);

                GameObject newEnemy;

                // da bi se u prve dve sobe stvarao samo prvi tip protivnika - verovatno menjati za sledece levele
                if (room.distanceFromStart < 3)
                {
                    enemies.Add(newEnemy = Instantiate(enemyPrefabs[0], spawnPos, Quaternion.identity, enemyParent));
                }
                else
                {
                    GameObject newPrefab = null;
                    do {
                        newPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                        
                    } while (newPrefab.GetComponent<Enemy>().difficulty > totalDifficulty);

                    newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity, enemyParent);
                    enemies.Add(newEnemy);
                }

                totalDifficulty -= newEnemy.GetComponent<Enemy>().difficulty;
            }
        }
    }

    protected virtual void CreateRooms(){
		//setup
		rooms = new Room[gridSizeX * 2,gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, Room.RoomType.START);
		takenPositions.Insert(0,Vector2.zero);
		Vector2 checkPos = Vector2.zero;
		//magic numbers
		float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
        //add rooms
		for (int i =0; i < numberOfRooms -1; i++){

			float randomPerc = ((float) i) / (((float)numberOfRooms - 1));
			randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
			//grab new position
			checkPos = NewPosition();
			//test new position
			if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare){
				int iterations = 0;
				do{
					checkPos = SelectiveNewPosition();
					iterations++;
				}while(NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
				if (iterations >= 50)
					print("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
			}
			//finalize position
			rooms[(int) checkPos.x + gridSizeX, (int) checkPos.y + gridSizeY] = new Room(checkPos, 0);
			takenPositions.Insert(0,checkPos);
		}

        checkPos = ObeliskPosition();

        if (NumberOfNeighbors(checkPos, takenPositions) > 1)
        {
            int iterations = 0;
            do
            {
                checkPos = ObeliskPosition();
                iterations++;
            } while (NumberOfNeighbors(checkPos, takenPositions) > 2 && iterations < 100);
            if (iterations >= 50)
                print("error: could not create obelisk room with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
        }

        rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.OBELISK);
        takenPositions.Insert(0, checkPos);

    }

	Vector2 NewPosition(){
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
        Vector2 neighborPos = Vector2.zero;

        do
        {
			int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // pick a random room
			x = (int) takenPositions[index].x;//capture its x, y position
			y = (int) takenPositions[index].y;

            neighborPos = takenPositions[index];

			bool UpDown = (Random.value < 0.5f);//randomly pick wether to look on hor or vert axis
			bool positive = (Random.value < 0.5f);//pick whether to be positive or negative on that axis
			if (UpDown){ //find the position bnased on the above bools
				if (positive){
					y += 1;
				}else{
					y -= 1;
				}
			}else{
				if (positive){
					x += 1;
				}else{
					x -= 1;
				}
			}
			checkingPos = new Vector2(x,y);
		}while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY || (takenPositions.Count > 1 && neighborPos.Equals(Vector2.zero))); //make sure the position is valid
		return checkingPos;
	}

    Vector2 ObeliskPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        Vector2 neighborPos = Vector2.zero;

        int i = 0;

        do
        {
            inc = 0;
            do
            {
                //instead of getting a room to find an adject empty space, we start with one that only 
                //as one neighbor. This will make it more likely that it returns a room that branches out
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 2 && inc < 100);
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            neighborPos = takenPositions[index];

            y += 1;

            checkingPos = new Vector2(x, y);

            i++;
        } while (i < 100 && (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY));
        if (inc >= 100)
        { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
            print("Error: could not find position with only one neighbor");
        }
        return checkingPos;
    }

	Vector2 SelectiveNewPosition(){ // method differs from the above in the two commented ways
		int index = 0, inc = 0;
		int x =0, y =0;
		Vector2 checkingPos = Vector2.zero;
        Vector2 neighborPos = Vector2.zero;
		do{
			inc = 0;
			do{ 
				//instead of getting a room to find an adject empty space, we start with one that only 
				//as one neighbor. This will make it more likely that it returns a room that branches out
				index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
				inc ++;
			}while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
			x = (int) takenPositions[index].x;
			y = (int) takenPositions[index].y;
            neighborPos = takenPositions[index];
			bool UpDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);
			if (UpDown){
				if (positive){
					y += 1;
				}else{
					y -= 1;
				}
			}else{
				if (positive){
					x += 1;
				}else{
					x -= 1;
				}
			}
			checkingPos = new Vector2(x,y);
		}while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY || (takenPositions.Count > 1 && neighborPos.Equals(Vector2.zero)));
		if (inc >= 100){ // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
			print("Error: could not find position with only one neighbor");
		}
		return checkingPos;
	}

	int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions){
		int ret = 0; // start at zero, add 1 for each side there is already a room
		if (usedPositions.Contains(checkingPos + Vector2.right)){ //using Vector.[direction] as short hands, for simplicity
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.left)){
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.up)){
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.down)){
			ret++;
		}
		return ret;
	}

	void DrawMap(){

		foreach (Room room in rooms){
			if (room == null){
				continue; //skip where there is no room
			}
			Vector2 drawPos = room.gridPos;

            switch (room.type)
            {
                case Room.RoomType.OBELISK:
                    room.Init(lastRoom, roomParent);
                    break;
                default:
                    room.Init(GetRandomPrefab(), roomParent);
                    break;
            }
            

		}
	}

    private GameObject GetRandomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

	void SetRoomDoors(){
		for (int x = 0; x < ((gridSizeX * 2)); x++){
			for (int y = 0; y < ((gridSizeY * 2)); y++){
				if (rooms[x,y] == null){
					continue;
				}

                if(rooms[x, y].type == Room.RoomType.OBELISK)
                {
                    rooms[x, y].doorBot = true;
                    rooms[x, y].doorTop = false;
                    rooms[x, y].doorLeft = false;
                    rooms[x, y].doorRight = false;

                    continue;
                }

				Vector2 gridPosition = new Vector2(x, y);
				if (y - 1 < 0){ //check above
					rooms[x, y].doorBot = false;
				}else{
					rooms[x, y].doorBot = (rooms[x, y - 1] != null && (rooms[x, y - 1].type == Room.RoomType.DEFAULT));
                }
				if (y + 1 >= gridSizeY * 2){ //check bellow
					rooms[x, y].doorTop = false;
                }
                else{
					rooms[x, y].doorTop = (rooms[x, y + 1] != null && (rooms[x, y + 1].type == Room.RoomType.DEFAULT || rooms[x, y + 1].type == Room.RoomType.OBELISK));

                }
                if (x - 1 < 0){ //check left
					rooms[x, y].doorLeft = false;

                }
                else
                {
					rooms[x, y].doorLeft = (rooms[x - 1,y] != null && (rooms[x - 1, y].type == Room.RoomType.DEFAULT));
                }
                if (x + 1 >= gridSizeX * 2){ //check right
					rooms[x, y].doorRight = false;

                }
                else
                {
					rooms[x, y].doorRight = (rooms[x + 1,y] != null && (rooms[x + 1, y].type == Room.RoomType.DEFAULT));
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
}
