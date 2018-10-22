using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class AttackingCharacter : MonoBehaviour {

    public Weapon[] weapons;
    public int equippedWeaponIndex;                                               // opremljeno oruzje igraca

    public float maxHealth = 25;

    public enum CharacterType { PLAYER, ENEMY }
    public CharacterType type;

    public CharacterVision vision;

    public float dashCooldown = 6;
    public float dashSpeed = 12;

    public Image healthBar;
    public Image nextAttackBar;

    protected GameObject nextAttackBG;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    protected enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING, DASHING }                     
    
    protected PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;

    protected int ignoreMask;
    protected ContactFilter2D colFilter;

    protected AIPath path;
    protected Seeker seeker;

    protected float normalSpeed = 6;

    public float maxDashRange = 4;

    //protected bool dashed = false;
    protected float timeToDash;
    protected Transform dashingAt = null;
    protected float maxRaycastDistance = 50;

    protected Vector2 approxPosition;

    protected GraphUpdateScene groundFreer;

    public virtual void Awake()
    {

        CM = GetComponent<CharacterMovement>();

        health = maxHealth;
        path = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();
    }

    public virtual void Start()
    {

        ignoreMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Characters"));

        colFilter.useLayerMask = true;
        colFilter.SetLayerMask(ignoreMask);

        if(vision == null)
            vision = transform.Find("Vision").GetComponent<CharacterVision>();

        normalSpeed = path.maxSpeed;

        nextAttackBG = nextAttackBar.transform.parent.gameObject;

        timeToDash = dashCooldown;

        //GetComponent<GraphUpdateScene>().Apply();

        approxPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        groundFreer = GameObject.Find("AIFreeGround").GetComponent<GraphUpdateScene>();
    }

    public void StopAttacking()
    {

        target = null;
        playerState = PlayerState.IDLE;
        weapons[equippedWeaponIndex].Stop();
    }

    public void Attack(Transform target)
    {

        if (this.target != null && target == this.target)                     // ako je target razlicit od trenutnog
            return;

        this.target = target;
        CM.MoveToPosition(target.position);

        playerState = PlayerState.CHASING_ENEMY;
        weapons[equippedWeaponIndex].Stop();
    }

    public void ChangeWeapon()
    {
        weapons[equippedWeaponIndex].gameObject.SetActive(false);
        equippedWeaponIndex = (equippedWeaponIndex + 1) % weapons.Length;
        weapons[equippedWeaponIndex].gameObject.SetActive(true);
    }

    public bool CanSee(Transform target, float range = Mathf.Infinity)
    {

        if (range == Mathf.Infinity && !weapons[equippedWeaponIndex].IsInRange(target))
            return false;

        range = maxRaycastDistance;

        Vector3 startCast = transform.position;
        Vector3 endCast = target.position;

        Debug.DrawRay(startCast, endCast - startCast);

        RaycastHit2D[] results = new RaycastHit2D[6];

        for (int i = 0; i < Physics2D.CircleCast(startCast, 0.2f, (endCast - startCast).normalized, colFilter, results, range); i++) // ako mu je protivnik vidljiv (od zidova/prepreka)
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
        playerState = PlayerState.WALKING;
        CM.MoveToPosition(new Vector3(pos.x, pos.y, transform.position.z));

        target = null;

        weapons[equippedWeaponIndex].Stop();
    }

    private float stopDashingAt;

    protected IEnumerator Dash(Vector3 to)
    {
        if (playerState == PlayerState.DASHING || timeToDash < dashCooldown)
            yield break;

        StopAttacking();
        stopDashingAt = 0;
        MoveToPosition(to);

        while (path.pathPending)
            yield return new WaitForEndOfFrame();

        if (playerState != PlayerState.WALKING)        // ako je u medjuvremenu stigao do destinacije
            yield break;

        playerState = PlayerState.DASHING;

        if (path.remainingDistance > maxDashRange)
        {
            stopDashingAt = path.remainingDistance - maxDashRange;
        }

        path.maxSpeed = dashSpeed;

        timeToDash = 0;
        yield return null;
    }

    protected IEnumerator Dash(Transform at)
    {
        yield return StartCoroutine(Dash(at.position + (transform.position - at.position).normalized * 1.5f));
        dashingAt = at;
        yield return null;
    }

    protected void UpdateSpaceAround(bool allocateNew = true)
    {
        groundFreer.transform.position = approxPosition + new Vector2(0, GetComponent<BoxCollider2D>().size.y / 2);
        groundFreer.GetComponent<BoxCollider2D>().size = GetComponent<BoxCollider2D>().size;
        AstarPath.active.AddWorkItem(() => groundFreer.Apply());

        if(allocateNew)
            AstarPath.active.AddWorkItem(() => GetComponent<GraphUpdateScene>().Apply());

        AstarPath.active.QueueGraphUpdates();
    }

    protected virtual void Update()
    {
        
        if (timeToDash < dashCooldown)
        {
            timeToDash += Time.deltaTime;
        }

        Vector2 newApproxPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        if (!newApproxPosition.Equals(approxPosition))
        {

            //AstarPath.active.UpdateGraphs(groundFreer.GetComponent<BoxCollider2D>().bounds);
            //UpdateSpaceAround();
            approxPosition = newApproxPosition;

        }

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

                        weapons[equippedWeaponIndex].StartAttacking(target);                  // krece da napada oruzjem
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

                    if ((stopDashingAt == 0 && path.reachedEndOfPath) || (Mathf.Approximately(path.velocity.x, 0) && Mathf.Approximately(path.velocity.y, 0)))      // ako je stigao do destinacije
                    {

                        path.maxSpeed = normalSpeed;

                        if (dashingAt)
                        {
                            if(weapons[equippedWeaponIndex].IsInRange(dashingAt))
                                dashingAt.GetComponent<AttackingCharacter>().TakeDamage(weapons[equippedWeaponIndex].damage, Vector3.zero);
                            Attack(dashingAt);
                            dashingAt = null;
                        }
                        else
                        {
                            playerState = PlayerState.IDLE;
                        }

                    }
                    else if (stopDashingAt > 0 && path.remainingDistance < stopDashingAt)
                    {

                        
                        path.maxSpeed = normalSpeed;

                        if (dashingAt)
                        {
                            Attack(dashingAt);
                            dashingAt = null;

                        }
                        else
                        {
                            playerState = PlayerState.WALKING;
                        }
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

        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack == 1)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
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
        UpdateSpaceAround(false);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        AttackingCharacter attChar = collision.gameObject.GetComponent<AttackingCharacter>();

        if(playerState == PlayerState.DASHING && attChar && attChar.type != type)
        {
            attChar.TakeDamage(weapons[equippedWeaponIndex].damage, Vector3.zero);
        }
    }

}
