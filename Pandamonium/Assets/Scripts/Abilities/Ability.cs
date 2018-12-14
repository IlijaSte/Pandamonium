using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour {

    public string abilityName;

    public float cooldown = 1;
    public float range = 5;
    public float damage = 1;

    public float manaCost = 5;

    public bool knockback = false;
    public float knockbackForce = 1;

    public Sprite buttonSprite;
    public Sprite bgSprite;

    protected float cdProgress = 0;

    public AbilityManager am;

    protected virtual void Start()
    {
        am = transform.parent.GetComponent<AbilityManager>();
        cdProgress = cooldown;
    }

    protected virtual void Cast(Vector2 fromPosition, Vector2 direction)
    {
        cdProgress = 0;
    }

	public virtual bool TryCast(Vector2 fromPosition, Vector2 direction)
    {
        if(cdProgress >= cooldown)
        {
            Cast(fromPosition, direction);
            return true;
        }

        return false;
    }

    protected virtual void Update()
    {
        if(cdProgress < cooldown)
        {
            cdProgress += Time.deltaTime;
            am.UpdateAbilityCooldown(this, cdProgress / cooldown);
        }
    }

    protected virtual Transform GetFacingEnemy()
    {

        List<Transform> visibleTargets = new List<Transform>();

        float lockRadius = range;

        float minDistance = Mathf.Infinity;
        Transform closestTarget = null;

        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, lockRadius, am.parent.ignoreMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Vector2 dirToTarget = (targetsInRadius[i].transform.position - transform.position).normalized;

            if (Vector2.Angle(am.parent.GetFacingDirection(), dirToTarget) < (am.parent as PlayerWithJoystick).lockAngle / 2)   // promeniti
            {
                float distance = Vector2.Distance(transform.position, targetsInRadius[i].transform.position);

                if (distance < minDistance)
                {
                    if (am.parent.CanSee(targetsInRadius[i].transform, distance))
                    {
                        minDistance = distance;
                        closestTarget = targetsInRadius[i].transform;
                    }
                }
            }


        }

        return closestTarget;
    }
}
