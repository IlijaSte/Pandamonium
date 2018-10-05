using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class World3DGenerator : MonoBehaviour {

    public Tilemap obstacleTilemap;
    public GameObject foregroundPrefab;

    public Transform walls;
    public GameObject wallColliderPrefab;

    public int scanRangeX;
    public int scanRangeZ;

    public void Generate()
    {
        for (int i = -scanRangeX; i <= scanRangeX; i++)
        {

            for (int j = -scanRangeZ; j <= scanRangeZ; j++)
            {

                if (obstacleTilemap.GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(wallColliderPrefab, new Vector3(i + 0.5f, walls.position.y, j + 0.5f), wallColliderPrefab.transform.rotation, walls);
                }

                Tilemap foregroundTilemap;

                if ((foregroundTilemap = obstacleTilemap.transform.parent.Find("ForegroundTilemap").GetComponent<Tilemap>()).GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(foregroundPrefab, new Vector3(i + 0.5f, walls.position.y, j + 0.5f), Quaternion.identity, foregroundTilemap.transform);
                }
            }
        }
    }
}
