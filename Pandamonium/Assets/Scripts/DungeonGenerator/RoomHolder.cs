using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomHolder : MonoBehaviour {

    public Tilemap groundTilemap;
    public Tilemap corridorTilemap;
    public Tilemap obstacleTilemap;

    public Transform topEdge;
    public Transform bottomEdge;
    public Transform leftEdge;
    public Transform rightEdge;

    private Tilemap acidTilemap;

    private Room context;

    public Vector3Int positionEndOfCorridor;

    //public void Init(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    public void Init(Room context)
    {
        this.context = context;
        acidTilemap = LevelGeneration.I.acidTilemap;

        DrawCorridors(context.doorTop, context.doorBot, context.doorLeft, context.doorRight);

        for (int i = Mathf.FloorToInt(transform.position.x - LevelGeneration.I.roomWidth / 2); i <= transform.position.x + LevelGeneration.I.roomWidth / 2; i++)
        {
            for (int j = Mathf.FloorToInt(transform.position.y - LevelGeneration.I.roomHeight / 2); j <= transform.position.y + LevelGeneration.I.roomHeight / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);

                Vector3Int groundTilePos = groundTilemap.WorldToCell(tilePos);

                if (groundTilemap.HasTile(groundTilePos))
                {
                    if (context.IsTileWalkable(tilePos))
                    {
                        acidTilemap.SetTile(tilePos, null);
                    }

                }
            }

        }
    }

    private void Awake()
    {
        
    }

    private void DrawCorridor(Vector2 start, Vector2 direction)
    {
        Vector3Int tilePos = new Vector3Int();

        while (Mathf.Abs(start.x - transform.position.x) <= LevelGeneration.I.roomWidth / 2 &&
                Mathf.Abs(start.y - transform.position.y) <= LevelGeneration.I.roomHeight / 2)
        {
            tilePos = groundTilemap.WorldToCell(start);

            TileBase prefab = ((direction.Equals(Vector2.left) || direction.Equals(Vector2.right)) ? LevelGeneration.I.corridorHorizPrefab : LevelGeneration.I.corridorVertPrefab);

            corridorTilemap.SetTile(tilePos, prefab);

            if(direction.Equals(Vector2.left) || direction.Equals(Vector2.right))      // corridor left / right
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(0, 1, 0), prefab);
                corridorTilemap.SetTile(tilePos + new Vector3Int(0, -1, 0), prefab);
            }
            else
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(1, 0, 0), prefab);
                corridorTilemap.SetTile(tilePos + new Vector3Int(-1, 0, 0), prefab);
            }

            acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);
            obstacleTilemap.SetTile(tilePos, null);

            start += direction;
        }

        positionEndOfCorridor = tilePos;

        start -= direction;

        acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);

        Vector3Int corridorPos = LevelGeneration.I.corridorTilemap.WorldToCell(start);

        if (direction.Equals(Vector2.up) || direction.Equals(Vector2.down))
        {

            LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgeVertPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(1, 0, 0), LevelGeneration.I.corridorBridgeVertPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(-1, 0, 0), LevelGeneration.I.corridorBridgeVertPrefab);

        }
        else
        {
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgeHorizPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, 1, 0), LevelGeneration.I.corridorBridgeHorizPrefab);
            LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, -1, 0), LevelGeneration.I.corridorBridgeHorizPrefab);

        }

    }

    public void DrawCorridors(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        if (doorTop)
        {
            DrawCorridor((Vector2)topEdge.position + new Vector2(0, -0.5f), Vector2.up); // proveriti za - 0.5f
        }
        if (doorBot)
        {
            DrawCorridor((Vector2)bottomEdge.position + new Vector2(0, 0.5f), Vector2.down); // proveriti za + 0.5f

        }
        if (doorLeft)
        {
            DrawCorridor((Vector2)leftEdge.position + new Vector2(0.5f, 0), Vector2.left); // proveriti za + 0.5f

        }
        if (doorRight)
        {
            DrawCorridor((Vector2)rightEdge.position + new Vector2(-0.5f, 0), Vector2.right); // proveriti za - 0.5f

        }

    }
}
