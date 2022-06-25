using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWSWeapon;

public class MonsterWeapon : Weapon
{
    private void Start()
    {
        SetRightHandTrigger();
        SetLeftHandTrigger();
    }

    void SetRightHandTrigger()
    {
        GetComponent<TriggerCallback>().collision_enter_event = RightMeleeAttacked;
    }
    void SetLeftHandTrigger()
    {
        if (melee_area.Length <= 1) return;
        
        melee_area[1].GetComponent<TriggerCallback>().collision_enter_event = LeftMeleeAttacked;
    }
    void RightMeleeAttacked(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") == true) 
        {
            int damage = GetDamageAppliedByMotion();

            Vector3 hit_pos = collider.ClosestPoint(melee_area[0].transform.position);
            collider.GetComponent<Player>().ReceiveDamage(damage, hit_pos);
        }
    }
    void LeftMeleeAttacked(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") == true)
        {
            int damage = GetDamageAppliedByMotion();

            Vector3 hit_pos = collider.ClosestPoint(melee_area[1].transform.position);
            collider.GetComponent<Player>().ReceiveDamage(damage, hit_pos);
        }
    }

    int GetDamageAppliedByMotion()
    {
        int defalut_damage = unit_stats.Total_Str;
        float ratio = 1f;

        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ratio = 1.2f;
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) ratio = 1.4f;

        return (int)(defalut_damage * ratio);
    }
}
