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
    private GameObject tapIndicator = null;

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

    protected IEnumerator ShowIndicator()
    {

        while (tapIndicator != null && Vector3.Distance(tapIndicator.transform.localScale, Vector3.one) > 0.2f)
        {

            tapIndicator.transform.localScale = Vector3.Lerp(tapIndicator.transform.localScale, Vector3.one, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }

    }

    public override void MoveToPosition(Vector3 pos)
    {
        base.MoveToPosition(pos);

        if (tapIndicator != null)
        {
            tapIndicator.transform.position = path.destination;
        }
        else
        {
            tapIndicator = Instantiate(tapIndicatorPrefab, path.destination, Quaternion.identity);
        }

        tapIndicator.transform.localScale = Vector3.zero;

        StartCoroutine(ShowIndicator());
    }

    // Update is called once per frame
    protected override void Update () {

        GameObject selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (playerState != PlayerState.DASHING && Input.GetMouseButton(0) && (selectedObject == null || (selectedObject && selectedObject.transform.parent.CompareTag("NonBlockableUI"))))
        {
            
            RaycastHit2D hit2D;

            if (hit2D = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y),
                                          Vector2.zero, 0f, ignoreMask | (1 << LayerMask.NameToLayer("Ground"))))
            {

                if (hit2D.transform.CompareTag("Enemy"))                // ako je kliknuo na neprijatelja
                {
                    if ((!clicked && !oneClick) || clicked)
                    {
                        oneClick = true;

                        doubleClickTimer = Time.time;
                        firstClickPos = hit2D.point;

                        base.Attack(hit2D.transform);
                    }
                    else if (!clicked && oneClick && Vector3.Distance(hit2D.point, firstClickPos) <= maxClickDistance)
                    {

                        oneClick = false;

                        StartCoroutine(Dash(hit2D.transform));
                    }
                }
                else if (hit2D.transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))    // ako je kliknuo na prepreke
                {
                    oneClick = false;

                }
                else if (hit2D.transform.CompareTag("Ground"))
                {

                    if ((!clicked && !oneClick) || clicked)
                    {
                        oneClick = true;

                        doubleClickTimer = Time.time;
                        firstClickPos = hit2D.point;

                        MoveToPosition(hit2D.point);
                    }
                    else if (!clicked && oneClick && Vector3.Distance(hit2D.point, firstClickPos) <= maxClickDistance)
                    {

                        oneClick = false;

                        // DASH
                        StartCoroutine(Dash(hit2D.point));
                    }
                }

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
        if(playerState == PlayerState.IDLE)
        {
            Destroy(tapIndicator);
            tapIndicator = null;

        }

        base.Update();
    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        base.TakeDamage(damage, dir);

        healthBar.fillAmount = health / maxHealth;
    }

    public override void Die()
    {
        //base.Die();
    }
}
