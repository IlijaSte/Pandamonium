using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidProjectile : MonoBehaviour {

    private Vector2 target;
    public float damage;

    public float speed = 50;

    private Rigidbody2D rb;
    private new CircleCollider2D collider;
    private Worm worm;
    private Transform indicator;
    private Animator animator;

    private Vector2 GetInitVelocity()
    {
        float g = -Physics2D.gravity.y;
        float x = target.x - transform.position.x;
        float y = target.y - transform.position.y;

        float b;
        float discriminant;
        speed -= 5;

        do
        {
            speed += 5;

            b = speed * speed - y * g;
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

    public void Shoot(Worm worm, Vector2 target, Transform indicator)
    {
        this.target = target;
        this.worm = worm;
        this.indicator = indicator;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
        collider.enabled = false;

        rb.AddForce(GetInitVelocity(), ForceMode2D.Impulse);
        
        if(target.x - transform.position.x < 0)
        {

            //animator.SetBool("Mirror", true);
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void Update()
    {
        if(Vector2.Distance(target, transform.position) < 0.5)
        {
            collider.enabled = true;
        }
        else
        {
            collider.enabled = false;
        }

        if(rb.velocity.y < 0 && transform.position.y <= target.y)
        {
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage, Vector3.zero);
            if (worm != null)
                player.TakeDamageOverTime(damage, 1, 3, worm.transform);
            else player.TakeDamageOverTime(damage, 1, 3);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
    }

}
