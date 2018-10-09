using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class AttackingCharacter : MonoBehaviour {

    public Weapon equippedWeapon;                                               // opremljeno oruzje igraca

    public float maxHealth = 25;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    protected enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING }                     
    
    protected PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;

    protected int ignoreMask;
    protected ContactFilter2D colFilter;

    protected AIPath path;

    public enum CharacterType { PLAYER, ENEMY }
    public CharacterType type;

    public virtual void Awake()
    {
        CM = GetComponent<CharacterMovement>();

        health = maxHealth;
        path = GetComponent<AIPath>();
    }

    public virtual void Start()
    {
        //ignoreMask = ~((1 << LayerMask.NameToLayer("Projectile")) | (1 << LayerMask.NameToLayer("Foreground")));
        ignoreMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Characters"));
        //ignoreMask = 0;
        colFilter.useLayerMask = true;
        colFilter.SetLayerMask(ignoreMask);
    }

    public void StopAttacking()
    {
        target = null;
        playerState = PlayerState.IDLE;
        equippedWeapon.Stop();
    }

    public void Attack(Transform target)
    {
        if (this.target == null || !this.target.Equals(target))                     // ako je target razlicit od trenutnog
        {

            this.target = target;

            CM.MoveToPosition(target.position);

            playerState = PlayerState.CHASING_ENEMY;

            equippedWeapon.Stop();
        }
    }

    protected bool CanSee(Transform target, float range = Mathf.Infinity)
    {

        Vector3 startCast = transform.position;
        Vector3 endCast = target.position;

        Debug.DrawRay(startCast, endCast - startCast);

        RaycastHit2D[] results = new RaycastHit2D[6];



        for (int i = 0; i < Physics2D.CircleCast(startCast, 0.1f, (endCast - startCast).normalized, colFilter, results, range); i++) // ako mu je protivnik vidljiv (od zidova/prepreka)
        {

            AttackingCharacter attChar = results[i].transform.GetComponent<AttackingCharacter>();
            if (attChar && attChar.type == type)
                continue;

            if(results[i].transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                return false;
            }

            if (results[i].transform == target)
            {
                return true;
            }
        }

        return false;

    }

    protected virtual void Update()
    {
        switch (playerState)
        {
            case PlayerState.CHASING_ENEMY:                                 // ako trenutno juri protivnika
                {

                    if (CanSee(target, (equippedWeapon.range == 0 ? 1.5f : equippedWeapon.range))) {

                        
                        equippedWeapon.StartAttacking(target);              // krece da napada oruzjem
                        playerState = PlayerState.ATTACKING;

                        CM.StopMoving();
                                
                    }
                    else
                    {
                        if (!path.destination.Equals(target.position))       // ako se protivnik u medjuvremenu pomerio
                        {
                            CM.MoveToPosition(target.position);

                        }
                    }

                    break;
                }
            case PlayerState.ATTACKING:
                {

                    if (target == null)                                         // ako je protivnik mrtav
                    {
                        StopAttacking();

                        break;
                    }

                    // ako mu je protivnik nestao iz weapon range-a
                    
                    if (!CanSee(target, (equippedWeapon.range == 0 ? 1.5f : equippedWeapon.range))) {

                        Transform tempTarget = target;
                        StopAttacking();
                        Attack(tempTarget);
                    }

                    break;
                }
        }
    }

    public virtual void TakeDamage(float damage, Vector3 dir)
    {
        if ((health -= damage) <= 0)    // * armorReduction
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
