using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{
    public override void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        base.ReceiveDamage(damage, hit_pos);
    }

    public override void DamagedAnimation()
    {

    }

    public override void ReportDeath()
    {
        death_callback(true);
    }
}
