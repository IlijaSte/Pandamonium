using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : HazardousArea<PlayerWithJoystick> {

    public float damageInterval;
    public float damage = 2;

    private void Update()
    {
        if(Time.time - lastDamage >= damageInterval)
        {
            DealDamage(damage);
        }
    }


    IEnumerator ColliderLifecycle(BoxCollider2D collider, float time)
    {
        yield return new WaitForSeconds(time);

        if (collider)
            Destroy(collider);

    }
    public void CreateCollider(Vector2 pos, float size, float time)
    {
        BoxCollider2D newCollider = gameObject.AddComponent<BoxCollider2D>();

        newCollider.size = new Vector2(size, size) / transform.lossyScale.x;
        newCollider.offset = pos;
        newCollider.usedByComposite = true;

        StartCoroutine(ColliderLifecycle(newCollider, time));
    }
}
