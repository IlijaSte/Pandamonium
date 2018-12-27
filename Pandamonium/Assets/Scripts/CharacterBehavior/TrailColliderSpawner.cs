using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColliderSpawner : MonoBehaviour {

    public float damage = 2;
    public float damageTick = 1;
    public float colliderSize;

    public float dmgStreakMultiplier = 5;

    public AttackingCharacter parent;

    private float time;
    private Vector2 lastPos;
    private CompositeCollider2D col;
    private TrailCollider tc;
    private GameObject parentObj;
    private GameObject childObj;

    private float minSize;
    private float maxSize;

    // Use this for initialization
    void Start () {

        lastPos = transform.position;

        //col = GetComponent<CompositeCollider2D>();

        TrailRenderer tr = GetComponent<TrailRenderer>();

        time = tr.time;
        //minSize = tr.widthCurve.keys[0].value;
        minSize = 0.5f;
        maxSize = tr.widthCurve.keys[tr.widthCurve.length - 1].value;

        parentObj = GameObject.Find("Trails");

        if (!parentObj)
        { 
            parentObj = new GameObject("Trails");
        }

        childObj = new GameObject("Trail");
        childObj.transform.SetParent(parentObj.transform);

        col = childObj.AddComponent<CompositeCollider2D>();
        col.isTrigger = true;

        Rigidbody2D rb = childObj.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        childObj.layer = LayerMask.NameToLayer("Hazardous");

        tc = childObj.AddComponent<TrailCollider>();
        tc.damage = damage;
        tc.damageInterval = damageTick;
        tc.streakMultiplier = dmgStreakMultiplier;

        if(parent == null)
        {
            parent = transform.parent.GetComponent<AttackingCharacter>();
        }
    }

    void CreateNewCollider(Vector2 position)
    {
        tc.CreateCollider(position, minSize, maxSize, time);
    }

	// Update is called once per frame
	void Update () {
		
        if((parent && parent.isDead))
        {
            parent = null;
            transform.SetParent(childObj.transform);
        }

        if(Vector2.Distance(lastPos, transform.position) > minSize)
        {
            lastPos = transform.position;

            CreateNewCollider(lastPos);
        }

	}

    private void OnDestroy()
    {
        if(col)
            Destroy(col.gameObject);
    }
}
