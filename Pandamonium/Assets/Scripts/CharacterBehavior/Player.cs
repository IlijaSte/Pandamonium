using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ponasanje igraca (kretanje, napad, ...)
public class Player : AttackingCharacter {

    public Image healthBar;

    public override void Start()
    {
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
                    agent.isStopped = false;
                    agent.stoppingDistance = 0;
                    agent.SetDestination(hit.point);
                    playerState = PlayerState.WALKING;

                    target3D = null;
                    
                    equippedWeapon.Stop();                                  // prestaje da napada oruzjem
                    
                }else if (hit.collider.CompareTag("Enemy"))                 // ako je korisnik kliknuo na *novog* protivnika, krece ka njemu
                {
                    base.Attack(hit.collider.transform);

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
