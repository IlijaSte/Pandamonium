using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDudeRock : AttackableObject {

    public float damageRadius = 0.5f;
    public float damage = 10;

    public GameObject indicatorPrefab;

    [HideInInspector]
    public Vector2 landPos;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool fallen = false;

    private GameObject indicator;

    protected override void Start()
    {

        base.Start();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        col.enabled = false;

        indicator = Instantiate(indicatorPrefab, landPos, Quaternion.identity);
        indicator.transform.localScale = new Vector2(damageRadius, damageRadius);
    }

    private void FixedUpdate()
    {
        if(!fallen && transform.position.y <= landPos.y)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;

            col.enabled = true;

            fallen = true;

            Destroy(indicator);

            if(Vector2.Distance(transform.position, GameManager.I.playerInstance.transform.position) <= damageRadius)
            {
                GameManager.I.playerInstance.TakeDamage(damage);
            }
        }
    }


}
