using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : ChannelingAbility {

    private ParticleSystem ps;
    private HazardousArea area;

    public float flameArcAngle = 60;

    protected override void Start()
    {
        base.Start();
        ps = GetComponent<ParticleSystem>();
        area = GetComponentInChildren<HazardousArea>();
        ParticleSystem.ShapeModule shape = ps.shape;

        shape.arc = flameArcAngle;
    }

    public override void StartChanneling()
    {
        base.StartChanneling();

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, am.parent.GetFacingDirection());
        area.transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90 - flameArcAngle / 2);

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.rotation = new Vector3(0, 0, rot.eulerAngles.z + 90 - flameArcAngle / 2);
        ps.Play();
        area.GetComponent<Collider2D>().enabled = true;
        //}
    }

    protected override void DoTick()
    {
        base.DoTick();

        if (!knockback)
        {
            area.DealDamage(damage);
        }
        else
        {
            area.DealDamageWithKnockback(damage, knockbackForce);
        }
    }

    protected override void Update()
    {

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, am.parent.GetFacingDirection());
        area.transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z + 90 - flameArcAngle / 2);

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.rotation = new Vector3(0, 0, rot.eulerAngles.z + 90 - flameArcAngle / 2);

        base.Update();
    }

    public override void StopChanneling()
    {
        base.StopChanneling();

        if (ps.isPlaying)
        {
            ps.Stop();
            
        }
        area.GetComponent<Collider2D>().enabled = false;
    }

}
