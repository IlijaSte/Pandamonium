using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    protected Rigidbody2D rb;

    protected Vector2 startPos;

    private float minBounceVelocity = 0.05f;

    private bool dropping = false;
    private int numBounces = 0;

    private Vector2 dropPos;

    private Vector2 dropDirection = Vector2.zero;

    private SpriteRenderer sprite;

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

        float distance = Vector2.Distance(transform.position, jumpTarget);

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

    public virtual void Drop()
    {
        startPos = transform.position;

        if (dropDirection.Equals(Vector2.zero))
            dropPos = startPos + Random.insideUnitCircle;
        else
            dropPos = startPos + dropDirection;

        rb.AddForce(GetInitVelocity(dropPos), ForceMode2D.Impulse);
        dropping = true;

        sprite.sortingOrder = -Mathf.RoundToInt(dropPos.y * 100);
    }

    protected virtual void FixedUpdate()
    {

        if(dropping && rb.velocity.y < 0 && transform.position.y <= dropPos.y)
        {
            if(numBounces < 5)
            {
                numBounces++;
                rb.AddForce(Vector2.up * -rb.velocity.y * 2, ForceMode2D.Impulse);
            }
            else
            {
                //rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                dropping = false;
                rb.drag = 10;

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

        if(collision.transform.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
        }
    }
}
