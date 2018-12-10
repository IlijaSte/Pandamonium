using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrb : Ability {

    public float projectileSpeed;

    public float aoeDamage = 2;
    public float aoeRadius = 2;

    private float realRadius = 2;

    public GameObject fireOrbPrefab;

    protected override void Cast(Vector2 fromPosition, Vector2 direction)
    {
        base.Cast(fromPosition, direction);

        // kreiranje projektila na mestu nosioca
        GameObject projectile = Instantiate(fireOrbPrefab);
        projectile.transform.position = fromPosition;

        realRadius = aoeRadius / projectile.transform.localScale.x;

        projectile.transform.GetComponentInChildren<CircleCollider2D>().radius = realRadius;

        // ispaljivanje projektila

        Transform facingEnemy = GetFacingEnemy();

        if (facingEnemy)
            projectile.GetComponent<FireOrbProjectile>().Shoot(this, facingEnemy);
        else
            projectile.GetComponent<FireOrbProjectile>().Shoot(this, direction);
    }

    public void ShowAoeIndicator(Vector2 position, GameObject prefab)
    {
        StartCoroutine(ShowIndicator(position, prefab));
    }

    public IEnumerator ShowIndicator(Vector2 position, GameObject prefab)
    {
        Transform indicator = Instantiate(prefab, position, Quaternion.identity).transform;

        indicator.localScale = new Vector2(realRadius * 2, realRadius * 2);

        SpriteRenderer sprite = indicator.GetComponent<SpriteRenderer>();

        Color endColor = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);

        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime;

            sprite.color = Color.Lerp(sprite.color, endColor, Time.deltaTime);

            yield return null;
        }

        Destroy(indicator.gameObject);
    }

}
