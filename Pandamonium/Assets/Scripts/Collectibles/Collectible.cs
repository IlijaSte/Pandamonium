using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public float playerPullDistance = 1f;

    protected Rigidbody2D rb;

    protected Vector2 startPos;

    //private float minBounceVelocity = 0.05f;

    private bool dropping = false;
    private int numBounces = 0;

    private Vector2 dropPos;

    private Vector2 dropDirection = Vector2.zero;

    private SpriteRenderer sprite;

    protected bool canPickup = false;

	protected virtual void Start () {

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        Drop();
	}

    public virtual void SetDropDirection(Vector2 dir)
    {
        dropDirection = dir;
    }

    private Vector2 GetInitVelocity(Vector2 jumpTarget)
    {
        float g = -Physics2D.gravity.y;
        float x = jumpTarget.x - transform.position.x;
        float y = jumpTarget.y - transform.position.y;

        float b;
        float discriminant;
        float jumpSpeed = 3;

        do
        {
            jumpSpeed += 1;

            b = jumpSpeed * jumpSpeed - y * g;
            discriminant = b * b - g * g * (x * x + y * y);

        } while (discriminant < 0);

        float discRoot = Mathf.Sqrt(discriminant);

        // Impact time for the most direct shot that hits.
        float T_min = Mathf.Sqrt((b - discRoot) * 2 / (g * g));

        // Impact time for the highest shot that hits.
        float T_max = Mathf.Sqrt((b + discRoot) * 2 / (g * g));

        float T = (T_max + T_min) / 2;

        float vx = x / T;
        float vy = y / T + T * g / 2;

        Vector2 velocity = new Vector2(vx, vy);

        return velocity;
    }

    protected void CanPickup()
    {
        canPickup = true;
    }

    public virtual void Drop()
    {
        startPos = transform.position;

        if (dropDirection.Equals(Vector2.zero))
            dropPos = startPos + Random.insideUnitCircle;
        else
            dropPos = startPos + dropDirection;

        rb.AddForce(GetInitVelocity(dropPos), ForceMode2D.Impulse);
        dropping = true;

        Invoke("CanPickup", 0.5f);

        sprite.sortingOrder = -Mathf.RoundToInt(dropPos.y * 100);

    }

    protected virtual IEnumerator GravitateTowardsPlayer()
    {

        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, GameManager.I.playerInstance.transform.position, Time.deltaTime);
            yield return null;
        }

    }

    protected virtual void FixedUpdate()
    {

        if(canPickup && Vector2.Distance(transform.position, GameManager.I.playerInstance.transform.position) <= playerPullDistance)
        {
            dropping = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().isTrigger = true;
            StartCoroutine(GravitateTowardsPlayer());
        }

        if(dropping && rb.velocity.y < 0 && transform.position.y <= dropPos.y)
        {
            if(numBounces < 5)
            {
                numBounces++;
                Vector2 newDropPos = (Vector2)transform.position + ((Vector2)transform.position - startPos).normalized * ((5f / numBounces) / 5f);
                Vector2 newVelocity = GetInitVelocity(newDropPos);
                startPos = transform.position;
                dropPos = newDropPos;
                rb.velocity = Vector2.zero;
                rb.AddForce(newVelocity, ForceMode2D.Impulse);
            }
            else
            {
                //rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                dropping = false;
                rb.drag = 10;
                rb.mass = 0.5f;
                gameObject.layer = LayerMask.NameToLayer("Default");       // !!!   da bi se collidovali sa chestom
                sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
            }
        }

    }

    protected virtual void OnPickup()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if(canPickup && collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
            print("collision");
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {

        if (canPickup && collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
            print("collision");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (canPickup && collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
            print("trigger");
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {

        if (canPickup && collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
            print("trigger");
        }
    }

}
