using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : HazardousArea<PlayerWithJoystick> {

    public float damageInterval;
    public float damage = 5;

    [HideInInspector]
    public float streakMultiplier = 5f;

    private bool inside = false;

    private int dmgStreak = 0;

    private void Update()
    {
        if(Time.time - lastDamage >= damageInterval)
        {
            DealDamage(damage + dmgStreak * streakMultiplier);
            dmgStreak++;
            lastDamage = Time.time;
        }
    }

    IEnumerator ColliderLifecycle(BoxCollider2D collider, float time, float minSize, float maxSize)
    {

        if (collider == null)
            yield break;

        Vector2 startSize = new Vector2(minSize, minSize);
        Vector2 endSize = new Vector2(maxSize, maxSize);

        for(int i = 0; i < time; i++)
        {
            collider.size = Vector2.Lerp(startSize, endSize, i / time);
            yield return new WaitForSeconds(1);
        }

        if (collider)
            Destroy(collider);

    }
    public void CreateCollider(Vector2 pos, float minSize, float maxSize, float time)
    {
        BoxCollider2D newCollider = gameObject.AddComponent<BoxCollider2D>();

        //newCollider.size = new Vector2(size, size) / transform.lossyScale.x;
        newCollider.offset = pos;
        newCollider.usedByComposite = true;

        StartCoroutine(ColliderLifecycle(newCollider, time, minSize, maxSize));
    }

    public override void DealDamage(float damage)
    {
        for (int i = 0; i < enemiesInArea.Count; i++)
        {
            PlayerWithJoystick enemy = enemiesInArea[i];

            if (enemy && !enemy.isDead && enemy.IsAttackable())
            {
                //enemy.TakePoisonDamage(damage);
                enemy.TakeDamageOverTime(damage, 1, 1);
            }
        }

    }

    protected override void OnCharacterEnter(PlayerWithJoystick character)
    {
        if (!inside)
        {
            character.speed = character.normalSpeed * 0.5f;
            lastDamage = Time.time;
            inside = true;
            dmgStreak = 0;
        }
    }

    protected override void OnCharacterExit(PlayerWithJoystick character)
    {
        if (inside)
        {
            character.speed = character.normalSpeed;
            inside = false;
            dmgStreak = 0;
        }
    }
}
