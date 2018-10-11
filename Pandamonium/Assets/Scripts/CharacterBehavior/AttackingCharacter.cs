using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class AttackingCharacter : MonoBehaviour {

    public Weapon equippedWeapon;                                               // opremljeno oruzje igraca

    public float maxHealth = 25;

    public enum CharacterType { PLAYER, ENEMY }
    public CharacterType type;

    public float visionRadius;

    public CharacterVision vision;

    public float dashSpeed = 12;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    protected enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING, DASHING }                     
    
    protected PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;

    protected int ignoreMask;
    protected ContactFilter2D colFilter;

    protected AIPath path;

    protected float normalSpeed = 6;

    public virtual void Awake()
    {

        CM = GetComponent<CharacterMovement>();

        health = maxHealth;
        path = GetComponent<AIPath>();
    }

    public virtual void Start()
    {

        ignoreMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Characters"));

        colFilter.useLayerMask = true;
        colFilter.SetLayerMask(ignoreMask);

        if(vision == null)
            vision = transform.Find("Vision").GetComponent<CharacterVision>();

        normalSpeed = path.maxSpeed;
    }

    public void StopAttacking()
    {

        target = null;
        playerState = PlayerState.IDLE;
        equippedWeapon.Stop();
    }

    public void Attack(Transform target)
    {

        if (this.target != null && target == this.target)                     // ako je target razlicit od trenutnog
            return;

        this.target = target;
        CM.MoveToPosition(target.position);

        playerState = PlayerState.CHASING_ENEMY;
        equippedWeapon.Stop();
    }

    public bool CanSee(Transform target, float range = Mathf.Infinity)
    {

        if (range == Mathf.Infinity && !equippedWeapon.IsInRange(target))
            return false;

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

    public void MoveToPosition(Vector3 pos)
    {
        CM.MoveToPosition(new Vector3(pos.x, pos.y, transform.position.z));

        playerState = PlayerState.WALKING;

        target = null;

        equippedWeapon.Stop();
    }

    protected void Dash(Vector3 to)
    {
        StopAttacking();
        MoveToPosition(to);
        playerState = PlayerState.DASHING;
        path.maxSpeed = dashSpeed;
    }

    protected virtual void Update()
    {
        switch (playerState)
        {
            case PlayerState.CHASING_ENEMY:                                     // ako trenutno juri protivnika
                {

                    if (target == null)                                         // ako je protivnik mrtav
                    {
                        StopAttacking();

                        break;
                    }

                    if (CanSee(target)) {

                        equippedWeapon.StartAttacking(target);                  // krece da napada oruzjem
                        playerState = PlayerState.ATTACKING;

                        CM.StopMoving();     
                    }
                    else
                    {
                        if (!path.destination.Equals(target.position))          // ako se protivnik u medjuvremenu pomerio
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

                    if (!CanSee(target)) {                                      // ako mu je protivnik nestao iz weapon range-a

                        Transform tempTarget = target;
                        StopAttacking();
                        Attack(tempTarget);
                    }

                    break;
                }

            case PlayerState.WALKING:
                {

                    if (!path.pathPending && (path.reachedEndOfPath || !path.hasPath))      // ako je stigao do destinacije
                    {

                        playerState = PlayerState.IDLE;

                    }

                    break;
                }

            case PlayerState.DASHING:
                {

                    if (!path.pathPending && (path.reachedEndOfPath || !path.hasPath))      // ako je stigao do destinacije
                    {

                        playerState = PlayerState.IDLE;
                        path.maxSpeed = normalSpeed;
                    }

                    break;
                }

            case PlayerState.IDLE:
                {
                    Player pl;
                    if ((pl = GetComponent<Player>()) && pl.oneClick)       // da ne bi doslo do utrkivanja DASH-a i ATTACK-a
                        break;

                    Transform closest;

                    if(closest = vision.GetClosest())
                    {
                        Attack(closest);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

        AttackingCharacter attChar;

        if(playerState == PlayerState.DASHING && (attChar = collision.gameObject.GetComponent<AttackingCharacter>()).type != type)
        {
            attChar.TakeDamage(equippedWeapon.damage, Vector3.zero);
        }
    }

}
