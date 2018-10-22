using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// osnovno ponasanje neprijatelja (vision, kretanje, napad)
public class Enemy : AttackingCharacter {

    protected Transform player;
    private static int numEnemies = 0;
    private BoardCreator boardCreator;

    public override void Start()
    {
        numEnemies ++;
        // ovde napraviti 3d objekat i dodeliti ga target-u CharacterMovement-a
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        healthBar = transform.Find("EnemyCanvas").Find("HealthBarBG").Find("HealthBar").GetComponent<Image>();

        type = CharacterType.ENEMY;

        boardCreator = FindObjectOfType<BoardCreator>();
    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);
        healthBar.fillAmount = health / maxHealth;

    }

    public override void Die()
    {
        numEnemies--;
        base.Die();
        if (boardCreator.isTutorial && numEnemies == 0)
            boardCreator.InstantiateFinishCollider();
    }

}
