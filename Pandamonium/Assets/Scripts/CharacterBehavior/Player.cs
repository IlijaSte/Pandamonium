using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

// ponasanje igraca (kretanje, napad, ...)
public class Player : AttackingCharacter {

    [HideInInspector]
    public bool oneClick = false;

    private float doubleClickTimer = 0;
    private float doubleClickDelay = 0.5f;
    private float maxClickDistance = 2f;
    private Vector3 firstClickPos;

    private bool clicked = false;

    public GameObject tapIndicatorPrefab;
    public GameObject attackIndicatorPrefab;
    private GameObject tapIndicator = null;

    private bool isDead = false;

    public override void Awake()
    {
        base.Awake();

        if (GameManager.joystick)
            gameObject.SetActive(false);
        else GameManager.I.playerInstance = this;
    }

    public override void Start()
    {
        type = CharacterType.PLAYER;

        foreach(Weapon weapon in weapons)
        {
            if (weapon != weapons[equippedWeaponIndex])
                weapon.gameObject.SetActive(false);
        }

        base.Start();

    }

    protected IEnumerator ShowIndicator(GameObject prefab, Transform parent = null)
    {

        if (tapIndicator != null)
        {

            Destroy(tapIndicator);
        }

        Vector2 instantiatePos = (parent == null ? path.destination : parent.position);

        tapIndicator = Instantiate(prefab, instantiatePos, Quaternion.identity, parent);
        tapIndicator.transform.localScale = Vector3.zero;

        while (tapIndicator != null && Vector3.Distance(tapIndicator.transform.localScale, Vector3.one) > 0.02f)
        {
            tapIndicator.transform.localScale = Vector3.Lerp(tapIndicator.transform.localScale, Vector3.one, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    public override void MoveToPosition(Vector3 pos)
    {
        base.MoveToPosition(pos);

        StartCoroutine(ShowIndicator(tapIndicatorPrefab));
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);
        StartCoroutine(ShowIndicator(attackIndicatorPrefab, target));
    }

    // Update is called once per frame
    protected override void Update () {


        switch (playerState)
        {

            case PlayerState.CHASING_ENEMY:
            case PlayerState.ATTACKING:

                if(target == null)      // ako je ubio protivnika
                {

                    StopAttacking();
                    Transform closest = vision.GetClosest();
                    if(closest != null)
                        Attack(closest);
                }
                break;

            case PlayerState.IDLE:

                Destroy(tapIndicator);
                tapIndicator = null;
                break;
        }

        base.Update();

        GameObject selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (playerState != PlayerState.DASHING && Input.GetMouseButton(0) && (selectedObject == null || (selectedObject != null && selectedObject.transform.parent.CompareTag("NonBlockableUI"))))
        {
            
            RaycastHit2D[] hit2D = new RaycastHit2D[3];

            hit2D = Physics2D.RaycastAll(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y),
                                          Vector2.zero, 0f, ignoreMask | (1 << LayerMask.NameToLayer("Ground")));
            foreach(RaycastHit2D hit in hit2D)
            {
                if (hit.transform.CompareTag("Player")) continue;

                if(hit.transform.GetComponent<TapDetector>() != null || hit.transform.CompareTag("Enemy"))
                //if (hit2D.transform.CompareTag("Enemy"))                // ako je kliknuo na neprijatelja
                {
                    if ((!clicked && !oneClick) || clicked)
                    {
                        oneClick = true;

                        doubleClickTimer = Time.time;
                        firstClickPos = hit.point;

                        Attack(hit.transform);
                    }
                    else if (!clicked && oneClick && Vector3.Distance(hit.point, firstClickPos) <= maxClickDistance)
                    {

                        oneClick = false;

                        //StartCoroutine(Dash(hit2D.transform));
                    }
                    
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))    // ako je kliknuo na prepreke
                {
                    oneClick = false;
                }
                //else if (hit2D.transform.CompareTag("Ground"))
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {

                    if ((!clicked && !oneClick) || clicked)
                    {
                        oneClick = true;

                        doubleClickTimer = Time.time;
                        firstClickPos = hit.point;

                        MoveToPosition(hit.point);
                    }
                    else if (!clicked && oneClick && Vector3.Distance(hit.point, firstClickPos) <= maxClickDistance)
                    {

                        oneClick = false;

                        // DASH
                        StartCoroutine(Dash(hit.point));
                    }
                }

                break;

            }

            clicked = true;

        }

        if (Input.GetMouseButtonUp(0))
        {
            clicked = false;
        }

        if(Time.time - doubleClickTimer > doubleClickDelay)
        {
            oneClick = false;
        }

        //if (!IsMoving())
    }

    public override void TakeDamage(float damage)
    {
        if (!isDead)
        {
            base.TakeDamage(damage);

            healthBar.fillAmount = health / maxHealth;
        }
    }

    public override void Die()
    {
        MenuManager.I.ShowMenu(MenuManager.I.deathMenu);
        isDead = true;
        //base.Die();
    }
}
