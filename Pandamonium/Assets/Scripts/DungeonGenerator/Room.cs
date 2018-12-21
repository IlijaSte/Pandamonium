using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room {
    public Vector2Int gridPos;

    public enum RoomType {DEFAULT, START, OBELISK, INTRO, KEY_HOLDER, BOSS, ELITE }
    public RoomType type;

	//public int type;
	public bool doorTop, doorBot, doorLeft, doorRight;

    public Transform instance;
    private RoomHolder roomHolder;
    public Tilemap corridorTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap groundTilemap;

    // Distance from the start room (in room lengths)
    public int distanceFromStart;

    public ArrayList enemies;

    public Vector2 spawnPoint;

    public Room nextTo;

    public Room(Vector2Int _gridPos, RoomType _type){
		gridPos = _gridPos;
		type = _type;
        enemies = new ArrayList();
	}

    public void Init(GameObject prefab, Transform parent)
    {
        this.instance = Object.Instantiate(prefab, new Vector2(gridPos.x * LevelGeneration.I.roomWidth, gridPos.y * LevelGeneration.I.roomHeight), Quaternion.identity, parent).transform;
        roomHolder = instance.GetComponent<RoomHolder>();

        corridorTilemap = roomHolder.corridorTilemap;
        obstacleTilemap = roomHolder.obstacleTilemap;
        groundTilemap = roomHolder.groundTilemap;

        roomHolder.Init(this);

        if(type == RoomType.START)
        {
            if (instance.Find("Portal"))
            {
                spawnPoint = (Vector2)instance.Find("Portal").position - new Vector2(0, 2);
            }
        }
    }

    public bool IsTileWalkable(Tilemap tilemap, Vector3 pos)
    {
        pos += new Vector3(0.45f, 0.45f);
        Vector3Int tilePos = tilemap.WorldToCell(pos);

        Vector2 checkPos = new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        if (tilemap.HasTile(tilePos) &&
            tilemap.HasTile(tilePos + new Vector3Int(1, 0, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(-1, 0, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(0, 1, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(0, -1, 0)) &&

            tilemap.HasTile(tilePos + new Vector3Int(-1, -1, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(-1, 1, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(1, -1, 0)) &&
            tilemap.HasTile(tilePos + new Vector3Int(1, 1, 0)))
            return true;

        return false;
    }

    public bool CanSpawnAtPos(Vector2 pos)
    {
        pos += new Vector2(0.45f, 0.45f);

        Vector2 checkPos = new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        if (!roomHolder.leftEdge.position.Equals(checkPos) &&
            !((Vector2)roomHolder.rightEdge.position + Vector2.left).Equals(checkPos) &&
            !roomHolder.topEdge.position.Equals(checkPos) &&
            !((Vector2)roomHolder.topEdge.position + Vector2.down).Equals(checkPos) &&
            !roomHolder.bottomEdge.position.Equals(checkPos) && 
            !((Vector2)roomHolder.leftEdge.position + Vector2.up).Equals(checkPos) && 
            !((Vector2)roomHolder.rightEdge.position + Vector2.up).Equals(checkPos))
            return true;

        return false;
    }

    public Vector2 GetSpawnPoint()
    {
        return roomHolder.GetFirstSpawnPoint();
    }

    public void PutEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public bool HasWalkableNeighbor(Tilemap tilemap, Vector3 pos)
    {
        pos += new Vector3(0.5f, 0.5f);
        Vector3Int tilePos = tilemap.WorldToCell(pos);

        if (!IsTileWalkable(tilemap, tilePos) &&
            (IsTileWalkable(tilemap, tilePos + new Vector3(1, 0, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(-1, 0, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(0, 1, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(0, -1, 0)) ||

            IsTileWalkable(tilemap, tilePos + new Vector3(-1, -1, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(-1, 1, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(1, -1, 0)) ||
            IsTileWalkable(tilemap, tilePos + new Vector3(1, 1, 0))))
            return true;

        return false;
    }

    public void PlaceDetail(Vector2 position, TileBase tile)
    {
        Vector3Int tilePos = roomHolder.detailTilemap.WorldToCell(position);
        roomHolder.detailTilemap.SetTile(tilePos, tile);
    }

    public Vector2 GetRandomPos()
    {

        Vector2 pos;

        do
        {

            pos = new Vector2(Random.Range(Mathf.RoundToInt(roomHolder.leftEdge.position.x), Mathf.RoundToInt(roomHolder.rightEdge.position.x)), Random.Range(Mathf.RoundToInt(roomHolder.bottomEdge.position.y), Mathf.RoundToInt(roomHolder.topEdge.position.y)));

            //pos = new Vector2(Mathf.Floor(pos.x) + 0.5f, Mathf.Floor(pos.y) + 0.5f);

        } while (!IsTileWalkable(groundTilemap, pos));

        pos = new Vector2(Mathf.Floor(pos.x) + 0.5f, Mathf.Floor(pos.y) + 0.5f);
        return pos;
    }

    public void LiftCorridors()
    {
        //corridorTilemap.SetTile()
    }

    public void LowerCorridors()
    {

    }
    
    public RoomHolder getRoomHolder()
    {
        return roomHolder;
    }
    
}
