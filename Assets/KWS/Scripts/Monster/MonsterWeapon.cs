using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWSWeapon;

public class MonsterWeapon : Weapon
{
    private void Start()
    {
        GetComponent<TriggerCallback>().collision_enter_event = MeleeAttacked;
    }
    void MeleeAttacked(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") == true) // 플레이어 태그
        {
            Vector3 hit_pos = collider.ClosestPoint(melee_area.transform.position);
            collider.GetComponent<Player>().ReceiveDamage(unit_stats.Total_Str, hit_pos);
        }
    }
}
