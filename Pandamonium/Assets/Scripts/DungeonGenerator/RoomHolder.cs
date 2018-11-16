using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomHolder : MonoBehaviour {

    public Tilemap groundTilemap;
    public Tilemap corridorTilemap;
    public Tilemap obstacleTilemap;
    public int width;
    public int height;

    public Transform topEdge;
    public Transform bottomEdge;
    public Transform leftEdge;
    public Transform rightEdge;

    private Tilemap acidTilemap;

    public void Init(bool doorTop, bool doorBot, bool doorLeft, bool doorRight)
    {
        acidTilemap = LevelGeneration.I.acidTilemap;

        for (int i = Mathf.FloorToInt(transform.position.x - width / 2); i <= transform.position.x + width / 2; i++)
        {
            for (int j = Mathf.FloorToInt(transform.position.y - height / 2); j <= transform.position.y + height / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);

                if (groundTilemap.HasTile(groundTilemap.WorldToCell(tilePos)))
                    acidTilemap.SetTile(tilePos, null);
            }

        }

        DrawCorridors(doorTop, doorBot, doorLeft, doorRight);
    }

    private void DrawCorridor(Vector2 start, Vector2 direction)
    {

        while (Mathf.Abs(start.x - transform.position.x) <= width / 2 &&
                Mathf.Abs(start.y - transform.position.y) <= height / 2)
        {
            Vector3Int tilePos = corridorTilemap.WorldToCell(start);
            corridorTilemap.SetTile(tilePos, LevelGeneration.I.corridorPrefab);
            obstacleTilemap.SetTile(tilePos, null);

            acidTilemap.SetTile(acidTilemap.WorldToCell(start), null);

            start += direction;
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
