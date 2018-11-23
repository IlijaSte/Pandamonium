using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room {
	public Vector2 gridPos;
	public int type;
	public bool doorTop, doorBot, doorLeft, doorRight;

    public Transform instance;
    private RoomHolder roomHolder;
    private Tilemap corridorTilemap;
    private Tilemap obstacleTilemap;
    private Tilemap groundTilemap;

    // Distance from the start room (in room lengths)
    public int distanceFromStart;

    public Room(Vector2 _gridPos, int _type){
		gridPos = _gridPos;
		type = _type;

        distanceFromStart = Mathf.RoundToInt(Mathf.Abs(_gridPos.x) + Mathf.Abs(_gridPos.y));
	}

    public void Init(GameObject prefab, Transform parent)
    {
        this.instance = Object.Instantiate(prefab, new Vector2(gridPos.x * LevelGeneration.I.roomWidth, gridPos.y * LevelGeneration.I.roomHeight), Quaternion.identity, parent).transform;
        roomHolder = instance.GetComponent<RoomHolder>();

        corridorTilemap = roomHolder.corridorTilemap;
        obstacleTilemap = roomHolder.obstacleTilemap;
        groundTilemap = roomHolder.groundTilemap;

        roomHolder.Init(this);

    }

    public bool IsTileWalkable(Vector3Int tilePos)
    {
        if (groundTilemap.HasTile(tilePos) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(1, 0, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(-1, 0, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(0, 1, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(0, -1, 0)) &&

            groundTilemap.HasTile(tilePos + new Vector3Int(-1, -1, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(-1, 1, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(1, -1, 0)) &&
            groundTilemap.HasTile(tilePos + new Vector3Int(1, 1, 0)))
            return true;

        return false;
    }

    public Vector2 GetRandomPos()
    {

        Vector2 pos;

        do
        {

            pos = new Vector2(Random.Range(roomHolder.leftEdge.position.x, roomHolder.rightEdge.position.x), Random.Range(roomHolder.bottomEdge.position.y, roomHolder.topEdge.position.y));

        } while (!IsTileWalkable(groundTilemap.WorldToCell(pos)));

        return (Vector3)pos;
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
