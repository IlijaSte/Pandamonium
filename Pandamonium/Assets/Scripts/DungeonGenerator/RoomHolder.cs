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
                    if (context.IsTileWalkable(groundTilePos))
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

        while (Mathf.Abs(start.x - transform.position.x) <= LevelGeneration.I.roomWidth / 2 &&
                Mathf.Abs(start.y - transform.position.y) <= LevelGeneration.I.roomHeight / 2)
        {
            Vector3Int tilePos = groundTilemap.WorldToCell(start);

            TileBase prefab = ((direction.Equals(Vector2.left) || direction.Equals(Vector2.right)) ? LevelGeneration.I.corridorHorizPrefab : LevelGeneration.I.corridorVertPrefab);

            corridorTilemap.SetTile(tilePos, prefab);

            if(direction.Equals(Vector2.left) || direction.Equals(Vector2.right))      // corridor left / right
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(0, 1, 0), prefab);
                //acidTilemap.SetTile(acidTilemap.WorldToCell(start) + new Vector3Int(0, 1, 0), null);

                corridorTilemap.SetTile(tilePos + new Vector3Int(0, -1, 0), prefab);
                //acidTilemap.SetTile(acidTilemap.WorldToCell(start) + new Vector3Int(0, -1, 0), null);

            }
            else
            {
                corridorTilemap.SetTile(tilePos + new Vector3Int(1, 0, 0), prefab);
                //acidTilemap.SetTile(acidTilemap.WorldToCell(start) + new Vector3Int(1, 0, 0), null);

                corridorTilemap.SetTile(tilePos + new Vector3Int(-1, 0, 0), prefab);
                //acidTilemap.SetTile(acidTilemap.WorldToCell(start) + new Vector3Int(-1, 0, 0), null);
            }

            obstacleTilemap.SetTile(tilePos, null);

            start += direction;
        }

        start -= direction;

        acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);

        Vector3Int corridorPos = LevelGeneration.I.corridorTilemap.WorldToCell(start);

        if (direction.Equals(Vector2.up) || direction.Equals(Vector2.down))
        {
            if (!LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(0, 1, 0)) &&
                !LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(0, -1, 0)))
            {
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgePrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(1, 0, 0), LevelGeneration.I.corridorBridgePrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(-1, 0, 0), LevelGeneration.I.corridorBridgePrefab);
            }

        }
        else
        {
            if (!LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(1, 0, 0)) &&
                !LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(-1, 0, 0)))
            {
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorBridgePrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, 1, 0), LevelGeneration.I.corridorBridgePrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, -1, 0), LevelGeneration.I.corridorBridgePrefab);
            }

        }

    }

    public void DrawCorridors(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        if (doorTop)
        {
            DrawCorridor((Vector2)topEdge.position, Vector2.up); // proveriti za - 0.5f
        }
        if (doorBot)
        {
            DrawCorridor((Vector2)bottomEdge.position, Vector2.down); // proveriti za + 0.5f

        }
        if (doorLeft)
        {
            DrawCorridor((Vector2)leftEdge.position, Vector2.left); // proveriti za - 0.5f

        }
        if (doorRight)
        {
            DrawCorridor((Vector2)rightEdge.position, Vector2.right); // proveriti za - 0.5f

        }

    }

    /*public void Update()
    {
        Debug.DrawLine(transform.position - new Vector3(width / 2, height / 2), transform.position + new Vector3(-width / 2, height / 2));
        Debug.DrawLine(transform.position + new Vector3(width / 2, -height / 2), transform.position + new Vector3(width / 2, height / 2));
        Debug.DrawLine(transform.position - new Vector3(width / 2, height / 2), transform.position + new Vector3(width / 2, -height / 2));
        Debug.DrawLine(transform.position + new Vector3(-width / 2, height / 2), transform.position + new Vector3(width / 2, height / 2));

    }*/
}
