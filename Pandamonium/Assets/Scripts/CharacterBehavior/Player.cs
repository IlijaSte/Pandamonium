using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

// ponasanje igraca (kretanje, napad, ...)
public class Player : AttackingCharacter {

    public Image healthBar;

    public override void Start()
    {
        type = CharacterType.PLAYER;
        base.Start();
    }

    public void MoveToPosition(Vector3 pos)
    {
        CM.MoveToPosition(new Vector3(pos.x, pos.y, transform.position.z));

        playerState = PlayerState.WALKING;

        target = null;

        equippedWeapon.Stop();
    }

    // Update is called once per frame
    protected override void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit2D hit2D;
            bool hitEnemy = false;

            if (hit2D = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y),
                                          Vector2.zero, 0f))
            {

                if (hit2D.transform.CompareTag("Enemy"))
                {
                    print("kliknuo na neprijatelja!");
                    base.Attack(hit2D.transform);
                    hitEnemy = true;
                }

            }
            if (!hitEnemy) { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("3D")))               // oprezno za 2D!!!
                {
                    if (hit.collider.CompareTag("3DGround"))                    // ako je korisnik kliknuo na zemlju, igrac krece ka toj poziciji
                    {

                        //agent.SetDestination(hit.point);
                        MoveToPosition(hit.point);

                    }
                }
            }
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
