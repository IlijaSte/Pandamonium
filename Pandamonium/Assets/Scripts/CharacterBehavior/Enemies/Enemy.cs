using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    protected Transform player;
    
    private bool spottedPlayer = false;
    protected bool dashed = false;

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
        if(!dashed && playerState == PlayerState.CHASING_ENEMY && CanSee(player, maxDashRange))
        {
            dashed = true;
            StartCoroutine(Dash(player.position + (transform.position - player.position).normalized * 1.5f ));
        }

        base.Update();

    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);
        healthBar.fillAmount = health / maxHealth;

    }

}
