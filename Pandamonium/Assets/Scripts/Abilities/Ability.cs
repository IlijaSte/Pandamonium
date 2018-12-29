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

    [HideInInspector]
    public float cdProgress = 0;

    public AbilityManager am;

    private AudioSource audioSource;

    protected virtual void Start()
    {
        am = transform.parent.GetComponent<AbilityManager>();
        cdProgress = cooldown;
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Cast(Vector2 fromPosition, Vector2 direction)
    {
        cdProgress = 0;
    }

    public virtual bool CanCast()
    {
        return (cdProgress >= cooldown && am.globalCDProgress >= am.globalCooldown);
    }

	public virtual bool TryCast(Vector2 fromPosition, Vector2 direction)
    {
        if(CanCast())
        {
            am.autolock.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));
            Cast(fromPosition, direction);
            if (audioSource)
                audioSource.Play();
            return true;
        }

        return false;
    }

    protected virtual void Update()
    {
        if(cdProgress < cooldown)
        {
            cdProgress += Time.deltaTime;
            //am.UpdateAbilityCooldown(this, cdProgress / cooldown);
            am.UpdateAbilityCooldown(this, cdProgress / cooldown);
        }

        if(am.globalCDProgress <= am.globalCooldown && am.globalCDProgress / am.globalCooldown < cdProgress / cooldown)
        {
            am.UpdateAbilityCooldown(this, am.globalCDProgress / am.globalCooldown);
        }

        if(cdProgress >= cooldown && am.globalCDProgress >= am.globalCooldown)
        {
            am.UpdateAbilityCooldown(this, 1);
        }
    }

    protected virtual Transform GetFacingEnemy()
    {

        return am.autolock.GetClosest();

        /*float lockRadius = range;

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

        return closestTarget;*/
    }
}
