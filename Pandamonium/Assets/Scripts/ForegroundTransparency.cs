using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForegroundTransparency : MonoBehaviour {

    private static Tilemap tilemap;
    public GameObject colliderPrefab;
    public int scanRangeX;
    public int scanRangeZ;

    private bool transparent = false;

    private float transpSpeed = 10;
    private Color nonTranspColor = new Color(1, 1, 1, 1);
    private Color transpColor = new Color(1, 1, 1, 0.35f);

    private Transform lastObject;
    private Vector3Int lastTile;

    private void Start()
    {

        if (tilemap == null)
            tilemap = GameObject.Find("/Grid/ForegroundTilemap").GetComponent<Tilemap>();

    }

    IEnumerator MakeTransparent(Vector3Int tile)
    {

        if (transparent) yield return null;

        transparent = true;
        tilemap.SetTileFlags(tile, TileFlags.None);

        float i = 0f;
        Color currColor = tilemap.GetColor(tile);

        while (i < 1f)
        {
            i += Time.deltaTime * transpSpeed;
            tilemap.SetColor(tile, Color.Lerp(currColor, transpColor, i));
            yield return new WaitForEndOfFrame();
        }


    }

    IEnumerator MakeNonTransparent(Vector3Int tile)
    {

        if (!transparent) yield return null;

        transparent = false;
        tilemap.SetTileFlags(tile, TileFlags.None);

        float i = 0f;
        Color currColor = tilemap.GetColor(tile);

        while (i < 1f)
        {
            i += Time.deltaTime * transpSpeed;
            tilemap.SetColor(tile, Color.Lerp(currColor, nonTranspColor, i));
            yield return new WaitForEndOfFrame();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            Vector3Int tile = tilemap.layoutGrid.WorldToCell(new Vector3(transform.position.x, tilemap.transform.position.y, transform.position.z));

            if (tilemap.HasTile(tile))
            {
                StartCoroutine(MakeTransparent(tile));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {

            Vector3Int tile = tilemap.layoutGrid.WorldToCell(new Vector3(transform.position.x, tilemap.transform.position.y, transform.position.z));

            lastTile = tile;
            lastObject = other.transform;

            if (tilemap.HasTile(tile))
            {

                AttackingCharacter attChar = other.transform.parent.GetComponent<AttackingCharacter>();

                if (!transparent)
                {
                    StartCoroutine(MakeTransparent(tile));

                }else if (tilemap.GetColor(tile).Equals(nonTranspColor)) {
                    transparent = false;
                    StartCoroutine(MakeTransparent(tile));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            Vector3Int tile = tilemap.layoutGrid.WorldToCell(new Vector3(transform.position.x, tilemap.transform.position.y, transform.position.z));

            if (tilemap.HasTile(tile))
            {
                StartCoroutine(MakeNonTransparent(tile));
            }
        }
    }

    private void Update()
    {
        if (lastObject == null && transparent)
        {
            StartCoroutine(MakeNonTransparent(lastTile));
        }
    }
}
