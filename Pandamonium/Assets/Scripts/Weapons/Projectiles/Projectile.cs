using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    protected Transform target;
    protected float speed;

    protected Transform parent;

    protected float damage;
    protected float range;

    protected bool shot = false;
    [HideInInspector]
    public Vector3 direction;

    public bool homing = false;

    private Vector2 startPos;
    protected bool knockback;
    protected float kbForce;

    public Sprite[] spritePool;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public virtual void Shoot(Transform parent, Transform target, float damage, float range, float speed, bool knockback = false, float kbForce = 0)
    {
        this.target = target;
        direction = (target.position - transform.position).normalized;
        this.knockback = knockback;
        this.kbForce = kbForce;
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        shot = true;
        this.parent = parent;

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

        if(spritePool.Length > 0)
        {
            GetComponent<SpriteRenderer>().sprite = spritePool[Random.Range(0, spritePool.Length)];
        }
    }

    public virtual void Shoot(Transform parent, Vector2 direction, float damage, float range, float speed, bool knockback = false, float kbForce = 0)
    {
        this.direction = direction;
        this.speed = speed;
        this.knockback = knockback;
        this.kbForce = kbForce;
        this.target = null;
        this.damage = damage;
        this.range = range;
        homing = false;

        shot = true;
        this.parent = parent;

        startPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

    }

    protected virtual void OnEndOfRange()
    {
        
    }

    protected virtual void Update()
    {
        if (!shot) return;

        if (homing && target == null)      // ako je target unisten/ubijen u medjuvremenu
        {
            homing = false;
            return;
        }

        if (homing)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
            transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90);

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, startPos) >= range)
            {
                OnEndOfRange();
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator PlaySound()
    {
        if (audioSource)
        {
            print("pustaj taj film");
            if (!audioSource.clip)
                print("nema zvuk");
            audioSource.Play();

        }
        else print("nije nasao projectile audio");

        yield return new WaitUntil(() => !audioSource.isPlaying);
    }

    protected virtual void OnHitEnemy(AttackingCharacter enemyHit)
    {
        StartCoroutine(PlaySound());

        if (knockback)
        {
            enemyHit.TakeDamageWithKnockback(damage, (enemyHit.transform.position - transform.position).normalized, kbForce);
        }
        else
        {
            enemyHit.TakeDamage(damage);
        }

        PlayerWithJoystick player;
        if (player = parent.GetComponent<PlayerWithJoystick>())
        {
            player.IncreaseEnergy(player.realDamage);
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (parent != null && other.gameObject == parent.gameObject)                   // ako je projektil pogodio pucaca
            return;

        AttackingCharacter character = other.GetComponent<AttackingCharacter>();

        if (character != null && parent != null && character.type != parent.GetComponent<AttackingCharacter>().type && character.IsAttackable())
        {

            OnHitEnemy(character);

            shot = false;
            Destroy(gameObject);

            return;
        }

        if (character != null && parent != null && character.type == parent.GetComponent<AttackingCharacter>().type)  // ako je pogodio karaktera istog tipa
        {
            return;
        }

        /*if(other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }*/

    }
}
