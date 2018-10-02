using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackingCharacter : MonoBehaviour {

    public GameObject object3DPrefab;

    public Weapon equippedWeapon;                                               // opremljeno oruzje igraca

    public float maxHealth = 25;

    protected Transform object3D;                                               // odgovarajuci objekat karaktera u 3D prostoru
    protected NavMeshAgent agent;

    protected Transform target3D = null;                                        // 3D objekat koji igrac napada/prati
    protected Transform target = null;                                          // 2D objekat koji igrac napada/prati

    protected enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING }    
                                                                                    
    protected PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;
    protected Transform world3D;

    protected int ignoreMask;

    public virtual void Awake()
    {
        world3D = GameObject.FindGameObjectWithTag("3DWorld").transform;
        CM = GetComponent<CharacterMovement>();

        object3D = Instantiate(object3DPrefab,
                               new Vector3(transform.position.x - CM.xOffset, world3D.position.y, transform.position.z - CM.zOffset),
                               Quaternion.identity, world3D).transform;

        CM.target = object3D;

        health = maxHealth;
    }

    public virtual void Start()
    {
        agent = object3D.GetComponent<NavMeshAgent>();
        ignoreMask = ~((1 << LayerMask.NameToLayer("Projectile")) | (1 << LayerMask.NameToLayer("Foreground")));
    }

    public void StopAttacking()
    {
        target3D = null;
        playerState = PlayerState.IDLE;
        equippedWeapon.Stop();
    }

    public void Attack(Transform target)
    {
        Transform temp = target.GetComponent<CharacterMovement>().target;
        if (target3D == null || !target3D.Equals(temp))                     // ako je target razlicit od trenutnog
        {

            agent.isStopped = false;
            print("Krenuo ka neprijatelju!");

            agent.stoppingDistance = 0f;        // !!!

            target3D = temp;
            this.target = target;
            agent.SetDestination(target3D.position);

            playerState = PlayerState.CHASING_ENEMY;

            equippedWeapon.Stop();
        }
    }

    protected virtual void Update()
    {
        switch (playerState)
        {
            case PlayerState.CHASING_ENEMY:                                 // ako trenutno juri protivnika
                {

                    if (Vector3.Distance(agent.destination, agent.transform.position) <= (equippedWeapon.range == 0 ? 1.5f : equippedWeapon.range))  // ako mu je protivnik u weapon range-u
                    {

                        Vector3 startCast = transform.position;
                        Vector3 endCast = target.position;

                        Ray ray = new Ray(startCast, endCast - startCast);

                        Debug.DrawRay(startCast, endCast - startCast);
                        RaycastHit hit;

                        if (Physics.SphereCast(ray, 0.2f, out hit, Mathf.Infinity, ignoreMask) && (hit.collider.transform == target)) // ako mu je protivnik vidljiv (od zidova/prepreka)
                        {
                            print("Stigao kod neprijatelja!");
                            agent.isStopped = true;
                            agent.velocity = Vector3.zero;
                            equippedWeapon.StartAttacking(target);              // krece da napada oruzjem
                            playerState = PlayerState.ATTACKING;
                        }
                    }
                    else
                    {
                        if (!agent.destination.Equals(target3D.position))       // ako se protivnik u medjuvremenu pomerio
                        {
                            agent.SetDestination(target3D.position);
                        }
                    }

                    break;
                }
            case PlayerState.ATTACKING:
                {

                    if (target == null || target3D == null)                                         // ako je protivnik mrtav
                    {
                        StopAttacking();

                        break;
                    }

                    // ako mu je protivnik nestao iz weapon range-a
                    if (Vector3.Distance(target3D.position, agent.transform.position) > (equippedWeapon.range == 0 ? 1.5f : equippedWeapon.range))
                    {
                        Transform tempTarget = target;
                        StopAttacking();
                        Attack(tempTarget);

                        break;
                    }

                    Vector3 startCast = transform.position;
                    Vector3 endCast = target.position;

                    Ray ray = new Ray(startCast, endCast - startCast);

                    Debug.DrawRay(startCast, endCast - startCast);
                    RaycastHit hit;

                    // ako vise ne vidi protivnika
                    if (!(Physics.SphereCast(ray, 0.2f, out hit, Mathf.Infinity, ignoreMask) && (hit.collider.transform == target))) // ako mu je protivnik vidljiv (od zidova/prepreka)
                    {
                        Transform tempTarget = target;
                        StopAttacking();
                        Attack(tempTarget);

                        break;
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
        Destroy(object3D.gameObject);
        Destroy(gameObject);
    }

}
