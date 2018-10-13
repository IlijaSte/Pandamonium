using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    protected Transform player;
    
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
        /*
        if (CanSee(player, visionRadius)) { 
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

}
