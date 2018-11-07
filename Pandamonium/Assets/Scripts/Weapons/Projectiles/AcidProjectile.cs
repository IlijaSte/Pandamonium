using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidProjectile : MonoBehaviour {

    private Vector2 target;
    public float damage;

    public float speed = 50;

    private Rigidbody2D rb;
    private Worm worm;

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

    public void Shoot(Worm worm, Vector2 target)
    {
        this.target = target;
        this.worm = worm;

        rb = GetComponent<Rigidbody2D>();

        rb.AddForce(GetInitVelocity(), ForceMode2D.Impulse);
        
    }

    public void Update()
    {
        if(rb.velocity.y < 0 && transform.position.y <= target.y)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage, Vector3.zero);
            player.TakeDamageOverTime(worm.transform, damage, 1, 3);
            Destroy(gameObject);
        }
    }

}
