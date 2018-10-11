using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

// ponasanje igraca (kretanje, napad, ...)
public class Player : AttackingCharacter {

    public Image healthBar;

    [HideInInspector]
    public bool oneClick = false;

    private float doubleClickTimer = 0;
    private float doubleClickDelay = 0.5f;
    private float maxClickDistance = 0.5f;
    private Vector3 firstClickPos;

    public override void Start()
    {
        type = CharacterType.PLAYER;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {

        if (playerState != PlayerState.DASHING && Input.GetMouseButtonDown(0))
        {
            
            RaycastHit2D hit2D;
            bool hitSmth = false;

            if (hit2D = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y),
                                          Vector2.zero, 0f, ignoreMask))
            {

                if (hit2D.transform.CompareTag("Enemy"))                // ako je kliknuo na neprijatelja
                {
                    base.Attack(hit2D.transform);
                    hitSmth = true;

                }else if(hit2D.transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))    // ako je kliknuo na prepreke
                {
                    hitSmth = true;
                }

            }
            if (!hitSmth) { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("3D")))               // oprezno za 2D!!!
                {
                    if (hit.collider.CompareTag("3DGround"))                    // ako je korisnik kliknuo na zemlju, igrac krece ka toj poziciji
                    {

                        if (!oneClick)
                        {
                            oneClick = true;
                            doubleClickTimer = Time.time;
                            firstClickPos = hit.point;
                            MoveToPosition(hit.point);
                        }
                        else if(Vector3.Distance(hit.point, firstClickPos) <= maxClickDistance)
                        {
                            
                            oneClick = false;

                            // DASH
                            Dash(hit.point);
                        }
                    }
                }
            }
            else
            {
                oneClick = false;
            }
        }

        if(Time.time - doubleClickTimer > doubleClickDelay)
        {
            oneClick = false;
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
