using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Monster
{
    [SerializeField] private string boss_name;
    [SerializeField] private Text boss_name_text; 
    
    new void Start()
    {
        Debug.Log("보스 스타트");
        base.Start();
        boss_name_text.text = boss_name;
    }
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
