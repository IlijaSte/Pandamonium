using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class ColliderGenerator : MonoBehaviour {

    public Tilemap obstacleTilemap;
    public GameObject foregroundPrefab;

    public Transform walls;
    public GameObject wallColliderPrefab;

    public void Generate(int scanRangeX, int scanRangeY)
    {
        for (int i = -scanRangeX; i <= scanRangeX; i++)
        {

            for (int j = -scanRangeY; j <= scanRangeY; j++)
            {

                if (obstacleTilemap.GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(wallColliderPrefab, new Vector3(i + 0.5f, j + 0.5f, walls.position.z), wallColliderPrefab.transform.rotation, walls);
                }

                Tilemap foregroundTilemap;

                if ((foregroundTilemap = obstacleTilemap.transform.parent.Find("ForegroundTilemap").GetComponent<Tilemap>()).GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(foregroundPrefab, new Vector3(i + 0.5f, j + 0.5f, walls.position.z), Quaternion.identity, foregroundTilemap.transform);
                }
            }
        }
    }
}
