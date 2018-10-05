using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    private Transform player;
    public float visionRadius;
    private bool spottedPlayer = false;

    private Image healthBar;

    public override void Start()
    {
        // ovde napraviti 3d objekat i dodeliti ga target-u CharacterMovement-a
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        healthBar = transform.Find("EnemyCanvas").Find("HealthBarBG").Find("HealthBar").GetComponent<Image>();

        type = CharacterType.ENEMY;
    }

    protected override void Update()
    {
        Vector3 startCast = transform.position;
        Vector3 endCast = player.position;


        Ray ray = new Ray(startCast, endCast - startCast);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.2f, out hit, visionRadius, ignoreMask) && hit.collider.CompareTag("Player")) // ako vidi igraca, krece da ga juri
        {

            base.Attack(player);
            spottedPlayer = true;

        }
        else
        {
            if (spottedPlayer)
            {
                base.StopAttacking();
                base.Attack(player);
            }
        }

        base.Update();

    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);
        healthBar.fillAmount = health / maxHealth;

    }
}
