using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour {
	public Vector2 worldSize = new Vector2(4,4);
	Room[,] rooms;
	List<Vector2> takenPositions = new List<Vector2>();
	int gridSizeX, gridSizeY, numberOfRooms = 20;
	//public GameObject roomWhiteObj;

    public GameObject[] roomPrefabs;
    public TileBase corridorPrefab;

    public TileBase groundPrefab;
    public Tilemap corridorTilemap;

    public Tilemap acidTilemap; 
    public TileBase acidPrefab;

    // public Tilemap corridorTilemap;
    private Transform roomParent;
    public int roomWidth;
    public int roomHeight;

    public Transform ground;

    public IntRange numEnemies;

    public GameObject[] enemyPrefabs;

    private static LevelGeneration instance;
    private Transform enemyParent;

    private int nextLayerOrder = 0;

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

        Random.InitState(System.Environment.TickCount);


        CreateRooms(); //lays out the actual map
        FillAcid();     // fills room with acid (or other obstacle type in the future)
		SetRoomDoors(); //assigns the doors where rooms would connect
		DrawMap(); //instantiates objects to make up a map

	}

    protected virtual IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        float nodeSize = AstarPath.active.data.gridGraph.nodeSize;
        AstarPath.active.data.gridGraph.SetDimensions(Mathf.RoundToInt((gridSizeX * roomWidth * 2 + roomWidth / 2) / nodeSize), Mathf.RoundToInt((gridSizeY * roomHeight * 2 + roomHeight / 2) / nodeSize), nodeSize);
        ground.localScale = new Vector2(gridSizeX * roomWidth * 2 + roomWidth / 2, gridSizeY * roomHeight * 2 + roomHeight / 2);

        Camera.main.GetComponent<CameraMovement>().SetBounds(-ground.localScale / 2, ground.localScale / 2);

        AstarPath.active.Scan();
        InstantiateEnemies();
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

    protected virtual void InstantiateEnemies()
    {

        int definiteNumEnemies = numEnemies.Random;

        for(int i = 0; i < definiteNumEnemies; i++)
        {
            Room room;
            do
            {
                Vector2 roomPos = takenPositions[Random.Range(1, takenPositions.Count)];
                room = rooms[gridSizeX + Mathf.RoundToInt(roomPos.x), gridSizeY + Mathf.RoundToInt(roomPos.y)];
            } while (room == GetRoomAtPos(GameManager.I.playerInstance.transform.position));
            Vector2 spawnPos = room.GetRandomPos();

            spawnPos = new Vector2(Mathf.Floor(spawnPos.x) + 0.5f, Mathf.Floor(spawnPos.y) + 0.5f);

            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity, enemyParent);
        }

    }

	protected virtual void CreateRooms(){
		//setup
		rooms = new Room[gridSizeX * 2,gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, 1);
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
	}

	Vector2 NewPosition(){
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do{
			int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // pick a random room
			x = (int) takenPositions[index].x;//capture its x, y position
			y = (int) takenPositions[index].y;
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
		}while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); //make sure the position is valid
		return checkingPos;
	}

	Vector2 SelectiveNewPosition(){ // method differs from the above in the two commented ways
		int index = 0, inc = 0;
		int x =0, y =0;
		Vector2 checkingPos = Vector2.zero;
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
		}while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
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

            room.Init(GetRandomPrefab(), roomParent, nextLayerOrder++);

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
				Vector2 gridPosition = new Vector2(x,y);
				if (y - 1 < 0){ //check above
					rooms[x,y].doorBot = false;
				}else{
					rooms[x,y].doorBot = (rooms[x,y-1] != null);
                }
				if (y + 1 >= gridSizeY * 2){ //check bellow
					rooms[x,y].doorTop = false;
                }
                else{
					rooms[x,y].doorTop = (rooms[x,y+1] != null);
                }
                if (x - 1 < 0){ //check left
					rooms[x,y].doorLeft = false;

                }
                else
                {
					rooms[x,y].doorLeft = (rooms[x - 1,y] != null);
                }
                if (x + 1 >= gridSizeX * 2){ //check right
					rooms[x,y].doorRight = false;

                }
                else
                {
					rooms[x,y].doorRight = (rooms[x+1,y] != null);
                }
            }
		}
	}
}
