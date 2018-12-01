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

    public float dashCooldown = 6;
    public float dashSpeed = 12;
    public float maxDashRange = 4;

    public float normalSpeed = 6;

    public Image healthBar;
    public Image nextAttackBar;

    public CharacterVision vision;

    protected GameObject nextAttackBG;

    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    public enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING, DASHING, IMMOBILE }                     
    
    [HideInInspector]
    public PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;

    [HideInInspector]
    public int ignoreMask;
    protected ContactFilter2D colFilter;

    protected AIPath path;

    

    //protected bool dashed = false;
    protected float timeToDash;
    protected Transform dashingAt = null;
    protected float maxRaycastDistance = 50;

    protected Vector2 approxPosition;
    protected Bounds currBounds;

    private ArrayList dotSources = new ArrayList();

    protected Rigidbody2D rb;

    protected SpriteRenderer sprite;

    [HideInInspector]
    public bool isDead = false;

    protected bool isKnockedBack = false;

    protected bool attackable = true;

    public bool IsAttackable()
    {
        return attackable;
    }

    public virtual void Awake()
    {

        CM = GetComponent<CharacterMovement>();
        path = GetComponent<AIPath>();
    }

    public virtual void Start()
    {

        ignoreMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Characters"));

        colFilter.useLayerMask = true;
        colFilter.SetLayerMask(ignoreMask);

        rb = GetComponent<Rigidbody2D>();

        if(vision == null)
            vision = transform.Find("Vision").GetComponent<CharacterVision>();

        if (path)
        {
            path.maxSpeed = normalSpeed;
        }

        health = maxHealth;

        nextAttackBG = nextAttackBar.transform.parent.gameObject;

        timeToDash = dashCooldown;

        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void StopAttacking()
    {

        target = null;
        playerState = PlayerState.IDLE;

        if(equippedWeaponIndex < weapons.Length)
            weapons[equippedWeaponIndex].Stop();
    }

    public void StopMoving()
    {
        CM.StopMoving();
    }

    public virtual void Attack(Transform target)
    {

        if (this.target != null && target == this.target)                     // ako je target razlicit od trenutnog
            return;

        if (playerState == PlayerState.IMMOBILE)
            return;

        this.target = target;
        CM.MoveToPosition(target.position);

        playerState = PlayerState.CHASING_ENEMY;
        weapons[equippedWeaponIndex].Stop();
    }

    public virtual void Attack() { }

    public void ChangeWeapon()
    {
        weapons[equippedWeaponIndex].gameObject.SetActive(false);
        equippedWeaponIndex = (equippedWeaponIndex + 1) % weapons.Length;
        weapons[equippedWeaponIndex].gameObject.SetActive(true);
    }

    public virtual Vector2 GetFacingDirection()
    {
        return rb.velocity;
    }

    // metoda proverava da li karakter vidi target na zadatom range-u (od drugih karaktera i obstacle-a)

    public bool CanSee(Transform target, float range = Mathf.Infinity)
    {

        if (range == Mathf.Infinity && !weapons[equippedWeaponIndex].IsInRange(target))
            return false;

        //range = maxRaycastDistance;

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

    // metoda za jednostavno pomeranje u tacku

    public virtual void MoveToPosition(Vector3 pos)
    {
        if (playerState != PlayerState.IMMOBILE)
        {
            playerState = PlayerState.WALKING;
            CM.MoveToPosition(new Vector3(pos.x, pos.y, transform.position.z));

            target = null;

            weapons[equippedWeaponIndex].Stop();
        }
    }

    private float stopDashingAt;

    protected IEnumerator Dash(Vector3 to)
    {
        if (playerState == PlayerState.DASHING || playerState == PlayerState.IMMOBILE || timeToDash < dashCooldown)
            yield break;

        timeToDash = 0;

        StopAttacking();
        stopDashingAt = 0;

        // krecemo da hodamo do cilja samo dok ne nadjemo celu putanju, nakon cega se ubrzavamo

        MoveToPosition(to);

        while (path.pathPending)
            yield return new WaitForEndOfFrame();

        if (playerState != PlayerState.WALKING)
            yield break;

        playerState = PlayerState.DASHING;

        if (path.remainingDistance > maxDashRange)
        {
            stopDashingAt = path.remainingDistance - maxDashRange;
        }

        path.maxSpeed = dashSpeed;

        
    }

    // za slucaj dash-ovanja na nesto umesto samo u tacku

    protected IEnumerator Dash(Transform at)
    {
        if (playerState != PlayerState.IMMOBILE)
        {
            yield return StartCoroutine(Dash(at.position + (transform.position - at.position) * 0.4f));

            // dashingAt cuva za kasnije (u Update kada zavrsi Dash) za damage-ovanje - izmeniti
            dashingAt = at;
        }
    }

    public virtual void Heal()
    {
        health = maxHealth;
        healthBar.fillAmount = 1;

        UIManager.I.ShowHeal(GetComponentInChildren<Canvas>(), 1);
    }

    protected void UpdateGraph()
    {

        /*Vector2 newApproxPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y)) + new Vector2(0.5f, 0.5f);

        if (!newApproxPosition.Equals(approxPosition))
        {

            GraphUpdateObject guo = new GraphUpdateObject(currBounds)
            {
                updatePhysics = true,
                modifyTag = true,
                setTag = 0
            };
            AstarPath.active.UpdateGraphs(guo);

            currBounds = new Bounds(newApproxPosition, Vector3.one);
            guo = new GraphUpdateObject(currBounds)
            {
                updatePhysics = true,
                modifyTag = true,
                setTag = (int)type + 1
            };
            AstarPath.active.UpdateGraphs(guo);

            approxPosition = newApproxPosition;

        }*/
    }

    protected virtual void Update()
    {

        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        if (isDead)
            return;

        if (timeToDash < dashCooldown)
        {
            timeToDash += Time.deltaTime;
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
                        if (!CM.destination.Equals(target.position))          // ako se protivnik u medjuvremenu pomerio
                        {
                            CM.MoveToPosition(target.position);
                        }
                    }

                    break;
                }

            case PlayerState.ATTACKING:
                {
                    Worm worm;
                    if (target == null || ((worm = target.GetComponent<Worm>()) != null && worm.state == Worm.WormState.BURIED))                                         // ako je protivnik mrtav
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
                                dashingAt.GetComponent<AttackingCharacter>().TakeDamage(weapons[equippedWeaponIndex].damage);
                            Attack(dashingAt);
                            dashingAt = null;
                        }
                        else
                        {
                            if (type == CharacterType.ENEMY && weapons[equippedWeaponIndex].IsInRange(GameManager.I.playerInstance.transform))
                            {
                                GameManager.I.playerInstance.TakeDamage(weapons[equippedWeaponIndex].damage);
                            }
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
                            if (type == CharacterType.ENEMY && weapons[equippedWeaponIndex].IsInRange(GameManager.I.playerInstance.transform))
                            {
                                GameManager.I.playerInstance.TakeDamage(weapons[equippedWeaponIndex].damage);
                            }
                            playerState = PlayerState.WALKING;
                        }
                    }
                    break;
                }
        }


        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack <= 0 || weapons[equippedWeaponIndex].timeToAttack == 1)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
        }

    }

    protected IEnumerator ColorTransition(Color color)
    {
        sprite.color = Color.red;

        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * 2;
            sprite.color = Color.Lerp(color, Color.white, i);
            yield return null;
        }
    }

    public virtual void TakeDamage(float damage)
    {

        if(GameManager.I.playerInstance == this)
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage, true);
        }
        else
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }
        

        if ((health -= damage) <= 0)    // * armorReduction
        {
            Die();
        }
        else
        {
            StartCoroutine(ColorTransition(Color.red));
        }
    }

    public virtual void TakePoisonDamage(float damage)
    {
        if (GameManager.I.playerInstance == this)
        {
            UIManager.I.ShowPoisonDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }
        else
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }


        if ((health -= damage) <= 0)    // * armorReduction
        {
            Die();
        }
        else
        {
            StartCoroutine(ColorTransition(Color.green));
        }
    }

    protected IEnumerator Knockback(Vector2 dir, float force)
    {
        isKnockedBack = true;
        PlayerState lastState = playerState;
        RigidbodyType2D lastType = rb.bodyType;

        playerState = PlayerState.IMMOBILE;

        if (path)
        {
            path.enabled = false;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        if (path)
        {
            path.enabled = true;
        }

        rb.bodyType = lastType;
        playerState = lastState;

        isKnockedBack = false;
    }

    public virtual void TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        TakeDamage(damage);

        if (playerState != PlayerState.DASHING && !isKnockedBack)
        {
            StartCoroutine(Knockback(dir, force));
        }

    }

    protected virtual IEnumerator DoT(Transform source, float damage, float interval, int times)
    {

        while(times-- > 0)
        {
            yield return new WaitForSeconds(interval);
            //TakeDamage(damage);
            TakePoisonDamage(damage);
           
        }

        if (dotSources.Contains(source))
        {
            dotSources.Remove(source);
        }
    }

    // interval - interval na koji ce igrac primati damage (u sekundama); timeInIntervals - trajanje DoT-a u intervalima
    public virtual void TakeDamageOverTime(float damage, float interval, int times, Transform source = null)
    {
        if (!dotSources.Contains(source))
        {
            dotSources.Add(source);
            StartCoroutine(DoT(source, damage, interval, times));
        }

        
    }

    public virtual IEnumerator Death()
    {

        StopAttacking();

        GetComponent<Collider2D>().enabled = false;
        healthBar.transform.parent.gameObject.SetActive(false);
        nextAttackBar.transform.parent.gameObject.SetActive(false);
        sprite.gameObject.SetActive(false);

        if(path)
            path.enabled = false;

        playerState = PlayerState.IMMOBILE;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

    public virtual void Die()
    {
        if (!isDead)
        {
            isDead = true;

            StartCoroutine(Death());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        /*AttackingCharacter attChar = collision.gameObject.GetComponent<AttackingCharacter>();

        if(playerState == PlayerState.DASHING && attChar && attChar.type != type)
        {
            attChar.TakeDamage(weapons[equippedWeaponIndex].damage, Vector3.zero);
        }*/
    }

    protected bool IsMoving()
    {

        return !(Mathf.Approximately(path.velocity.x, 0) && Mathf.Approximately(path.velocity.y, 0));

    }

}
