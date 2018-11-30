using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    protected Rigidbody2D rb;

    protected Vector2 startPos;

    private float minBounceVelocity = 0.05f;
    private float bounceForce = 1f;

    private bool dropping = false;
    private int numBounces = 0;

    private Vector2 dropPos;

	protected virtual void Start () {

        rb = GetComponent<Rigidbody2D>();
        Drop();
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

    protected virtual void Drop()
    {
        startPos = transform.position;

        dropPos = startPos + Random.insideUnitCircle;

        rb.AddForce(GetInitVelocity(dropPos), ForceMode2D.Impulse);
        dropping = true;
    }

    protected virtual void FixedUpdate()
    {

        if(dropping && rb.velocity.y < 0 && transform.position.y <= dropPos.y)
        {
            if(rb.velocity.magnitude > minBounceVelocity && numBounces < 5)
            {
                numBounces++;
                rb.AddForce(Vector2.up * bounceForce * (5 / numBounces), ForceMode2D.Impulse);
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;
                dropping = false;
            }
        }
    }

    protected virtual void OnPickup()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<AttackingCharacter>() == GameManager.I.playerInstance)
        {
            OnPickup();
        }
    }
}
