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

    public void Init(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        acidTilemap = LevelGeneration.I.acidTilemap;

        for (int i = Mathf.FloorToInt(transform.position.x - LevelGeneration.I.roomWidth / 2); i <= transform.position.x + LevelGeneration.I.roomWidth / 2; i++)
        {
            for (int j = Mathf.FloorToInt(transform.position.y - LevelGeneration.I.roomHeight / 2); j <= transform.position.y + LevelGeneration.I.roomHeight / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);

                Vector3Int groundTilePos = groundTilemap.WorldToCell(tilePos);

                if (groundTilemap.HasTile(groundTilePos))
                {
                    if(Room.IsTileWalkable(groundTilemap, groundTilePos))
                    {
                        acidTilemap.SetTile(tilePos, null);
                    }
                    
                }
            }

        }

        DrawCorridors(doorTop, doorBot, doorLeft, doorRight);
    }

    private void DrawCorridor(Vector2 start, Vector2 direction)
    {

        while (Mathf.Abs(start.x - transform.position.x) <= LevelGeneration.I.roomWidth / 2 &&
                Mathf.Abs(start.y - transform.position.y) <= LevelGeneration.I.roomHeight / 2)
        {
            Vector3Int tilePos = groundTilemap.WorldToCell(start);

            groundTilemap.SetTile(tilePos, LevelGeneration.I.groundPrefab);

            if(direction.Equals(Vector2.left) || direction.Equals(Vector2.right))      // corridor left / right
            {
                groundTilemap.SetTile(tilePos + new Vector3Int(0, 1, 0), LevelGeneration.I.groundPrefab);
                groundTilemap.SetTile(tilePos + new Vector3Int(0, -1, 0), LevelGeneration.I.groundPrefab);

            }
            else
            {
                groundTilemap.SetTile(tilePos + new Vector3Int(1, 0, 0), LevelGeneration.I.groundPrefab);
                groundTilemap.SetTile(tilePos + new Vector3Int(-1, 0, 0), LevelGeneration.I.groundPrefab);
            }

            obstacleTilemap.SetTile(tilePos, null);

            acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);

            start += direction;
        }

        start -= direction;

        Vector3Int corridorPos = LevelGeneration.I.corridorTilemap.WorldToCell(start);

        if (direction.Equals(Vector2.up) || direction.Equals(Vector2.down))
        {
            if (!LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(0, 1, 0)) &&
                !LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(0, -1, 0)))
            {
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorPrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(1, 0, 0), LevelGeneration.I.corridorPrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(-1, 0, 0), LevelGeneration.I.corridorPrefab);
            }

        }
        else
        {
            if (!LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(1, 0, 0)) &&
                !LevelGeneration.I.corridorTilemap.HasTile(corridorPos + new Vector3Int(-1, 0, 0)))
            {
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos, LevelGeneration.I.corridorPrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, 1, 0), LevelGeneration.I.corridorPrefab);
                LevelGeneration.I.corridorTilemap.SetTile(corridorPos + new Vector3Int(0, -1, 0), LevelGeneration.I.corridorPrefab);
            }

        }

    }

    public void DrawCorridors(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        if (doorTop)
        {
            DrawCorridor(topEdge.position, Vector2.up); // proveriti za - 0.5f
        }
        if (doorBot)
        {
            DrawCorridor(bottomEdge.position, Vector2.down); // proveriti za + 0.5f

        }
        if (doorLeft)
        {
            DrawCorridor(leftEdge.position, Vector2.left); // proveriti za - 0.5f

        }
        if (doorRight)
        {
            DrawCorridor(rightEdge.position, Vector2.right); // proveriti za - 0.5f

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
