using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {

    protected  Canvas minimapCanvas;
    public GameObject blackRectanglePrefab;

    protected List<Room> roomsToExplore;

    protected Room currRoom;

    protected List<GameObject> rectangles;

    protected Transform parent;

	// Use this for initialization
	void Start () {

        parent = new GameObject("MinimapRectHolder").transform;

        if(minimapCanvas == null)
        {
            minimapCanvas = UIManager.I.minimapCanvas;
        }

        roomsToExplore = new List<Room>();

        foreach (Vector2 pos in LevelGeneration.I.takenPositions)
        {
            roomsToExplore.Add(LevelGeneration.I.GetRoomAtGridPos(pos));
        }

        roomsToExplore.RemoveAt(roomsToExplore.Count - 1);      // prva soba je odmah otkrivena

        CreateRectangles();

	}

    private void CreateRectangles()
    {

        rectangles = new List<GameObject>();

        foreach(Room room in roomsToExplore)
        {
            GameObject newRect = Instantiate(blackRectanglePrefab, room.getRoomHolder().transform.position, Quaternion.identity, parent);
            newRect.transform.localScale = new Vector2(LevelGeneration.I.roomWidth, LevelGeneration.I.roomHeight);
            rectangles.Add(newRect);
            
        }

        for(int i = -LevelGeneration.I.gridSizeX; i < LevelGeneration.I.gridSizeX; i++)
        {
            for(int j = -LevelGeneration.I.gridSizeY; j < LevelGeneration.I.gridSizeY; j++)
            {
                if(LevelGeneration.I.GetRoomAtGridPos(new Vector2(i, j)) == null)
                {
                    GameObject newRect = Instantiate(blackRectanglePrefab, new Vector2(i * LevelGeneration.I.roomWidth, j * LevelGeneration.I.roomHeight), Quaternion.identity, parent);
                    newRect.transform.localScale = new Vector2(LevelGeneration.I.roomWidth, LevelGeneration.I.roomHeight);
                    rectangles.Add(newRect);
                }
            }
        }
    }

    IEnumerator RevealRoom(Room room)
    {
        int index = roomsToExplore.IndexOf(room);
        roomsToExplore.RemoveAt(index);

        float i = 0;
        SpriteRenderer rectSprite = rectangles[index].GetComponent<SpriteRenderer>();
        Color startColor = rectSprite.color;
        Color endColor = new Color(0, 0, 0, 0);

        while (i < 1)
        {
            i += Time.deltaTime;

            rectSprite.color = Color.Lerp(startColor, endColor, i);

            yield return null;
        }
        
        Destroy(rectangles[index]);
        rectangles.RemoveAt(index);
    }

    private void Update()
    {
        Room playerRoom = (GameManager.I.playerInstance as PlayerWithJoystick).GetRoom();
        if (playerRoom != null && roomsToExplore.Contains(playerRoom))
        {
            StartCoroutine(RevealRoom(playerRoom));
            //rectangles
        }

    }

}
