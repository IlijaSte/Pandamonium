using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class World3DGenerator : MonoBehaviour {

    public Tilemap obstacleTilemap;
    public NavMeshSurface surface;
    public GameObject instantiatePrefab;
    public GameObject foregroundPrefab;

    public Transform walls;
    public GameObject wallColliderPrefab;

    public int scanRangeX;
    public int scanRangeZ;

	// Use this for initialization
	void Awake () {

        // skenira u datom (x, z) range-u za tile-ove iz datog tilemap-a (zidovi za sad) i na odgovarajucem mestu u 3D prostoru pravi objekat po datom prefab-u

        // izmeniti da ide odozdo na gore, s leva udesno i ne stavlja collider na najnizu prepreku

        for(int i = -scanRangeX; i <= scanRangeX; i++)
        {

            for(int j = -scanRangeZ; j <= scanRangeZ; j++)
            {

                if(obstacleTilemap.GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(instantiatePrefab, new Vector3(i + 0.5f, transform.position.y, j + 0.5f), Quaternion.identity, transform);
                    Instantiate(wallColliderPrefab, new Vector3(i + 0.5f, walls.position.y, j + 0.5f), Quaternion.identity, walls);

                }

                Tilemap foregroundTilemap;

                if((foregroundTilemap = obstacleTilemap.transform.parent.Find("ForegroundTilemap").GetComponent<Tilemap>()).GetTile(new Vector3Int(i, j, 0)))
                {
                    Instantiate(foregroundPrefab, new Vector3(i + 0.5f, walls.position.y, j + 0.5f), Quaternion.identity, foregroundTilemap.transform);

                }

            }

        }

        surface.BuildNavMesh();
	}

}
