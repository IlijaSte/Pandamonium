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

    // Update is called once per frame
    protected override void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreMask))
            {
                if (hit.collider.CompareTag("3DGround"))                    // ako je korisnik kliknuo na zemlju, igrac krece ka toj poziciji
                {

                    //agent.SetDestination(hit.point);
                    CM.MoveToPosition(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                    
                    playerState = PlayerState.WALKING;

                    target = null;
                    
                    equippedWeapon.Stop();                                  // prestaje da napada oruzjem
                    
                }else if (hit.collider.CompareTag("Enemy"))                 // ako je korisnik kliknuo na *novog* protivnika, krece ka njemu
                {
                    print("kliknuo na neprijatelja!");
                    base.Attack(hit.collider.transform);

                }
            }
        }

        print(playerState.ToString());

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
