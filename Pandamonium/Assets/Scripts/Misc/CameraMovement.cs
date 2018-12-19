using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour {

    public Vector3 stageLowerLeft;
    public Vector3 stageUpperRight;

    private Transform player;

    private float minVisibleX;
    private float maxVisibleX;
    private float minVisibleY;
    private float maxVisibleY;

    private Vector2 lowerLeft;
    private Vector2 upperRight;
    float viewportWidth;
    float viewportHeight;

    //public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera eventCam;

    private static readonly int PRIORITY_ACTIVE = 15;
    private static readonly int PRIORITY_INACTIVE = -1;

    private Queue<CinemachineVirtualCamera> lookAts;

    private CinemachineVirtualCamera currCam;

    // Use this for initialization
    void Start () {
        lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        viewportWidth = upperRight.x - lowerLeft.x;
        viewportHeight = upperRight.y - lowerLeft.y;

        minVisibleX = stageLowerLeft.x + viewportWidth / 2f;
        maxVisibleX = stageUpperRight.x - viewportWidth / 2f;
        minVisibleY = stageLowerLeft.y + viewportHeight / 2f;
        maxVisibleY = stageUpperRight.y - viewportHeight / 2f;

        if(GameManager.I.playerInstance)
            player = GameManager.I.playerInstance.transform;

        lookAts = new Queue<CinemachineVirtualCamera>();
    }
	
    public void SetBounds(Vector2 lowerLeft, Vector2 upperRight)
    {
        this.stageLowerLeft = lowerLeft;
        this.stageUpperRight = upperRight;

        minVisibleX = stageLowerLeft.x + viewportWidth / 2f;
        maxVisibleX = stageUpperRight.x - viewportWidth / 2f;
        minVisibleY = stageLowerLeft.y + viewportHeight / 2f;
        maxVisibleY = stageUpperRight.y - viewportHeight / 2f;
    }

    IEnumerator Peek(CinemachineVirtualCamera at)
    {
        //eventCam.Follow = at;
        //currCam.Priority = PRIORITY_INACTIVE;
        //currCam = at;
        at.Priority = PRIORITY_ACTIVE;

        yield return new WaitForSeconds(2f);

        at.Priority = PRIORITY_INACTIVE;

        CinemachineVirtualCamera newT = lookAts.Dequeue();

        PeekFromQueue();

    }

    public void PeekFromQueue()
    {
        if (lookAts.Count == 0)
            return;

        CinemachineVirtualCamera newT = lookAts.Peek();

        if (newT)
        {
            StartCoroutine(Peek(newT));
        }

    }

    public void PeekAt(CinemachineVirtualCamera at)
    {
        lookAts.Enqueue(at);

        if(lookAts.Count == 1)
            StartCoroutine(Peek(at));
    }

	// Update is called once per frame
	void Update () {

        /*if(player == null)
            player = GameManager.I.playerInstance.transform;

        transform.position = new Vector3(Mathf.Clamp(player.position.x, minVisibleX, maxVisibleX), Mathf.Clamp(player.position.y, minVisibleY, maxVisibleY), transform.position.z);
	    */


    }
}
