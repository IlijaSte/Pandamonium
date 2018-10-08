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

        //print(Physics2D.CircleCast(startCast, 0.02f, (endCast - startCast).normalized, visionRadius, ignoreMask).collider.name);
        RaycastHit2D[] results = new RaycastHit2D[2];
        // ako vise ne vidi protivnika           

        for (int i = 0; i < Physics2D.CircleCast(startCast, 0.01f, (endCast - startCast).normalized, colFilter, results, Mathf.Infinity); i++) // ako mu je protivnik vidljiv (od zidova/prepreka)
        {
            AttackingCharacter attChar = results[i].transform.GetComponent<AttackingCharacter>();
            if (attChar && attChar.type == CharacterType.ENEMY)
                continue;

            if(results[i].transform == player)
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
            break;
        }
        /*if ((hit = Physics2D.CircleCast(startCast, 0.02f, (endCast - startCast).normalized, visionRadius, ignoreMask)) && hit.collider.CompareTag("Player")) // ako vidi igraca, krece da ga juri
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
        }*/

        base.Update();

    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);
        healthBar.fillAmount = health / maxHealth;

    }

    public void OnMouseDown()
    {
        player.GetComponent<Player>().Attack(transform);
    }

}
