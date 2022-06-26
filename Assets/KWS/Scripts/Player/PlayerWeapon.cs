using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWSWeapon;

public class PlayerWeapon : Weapon
{
    [SerializeField]
    private TriggerCallback trigger_callback;
    private void Start()
    {
        trigger_callback.collision_enter_event = MeleeAttacked;
    }

    public void MeleeAttacked(Collider collider)
    {
        if (collider.gameObject.CompareTag("Monster") == true) // 몬스터 태그
        {
            float ratio = 1f;

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true) { ratio = 1.3f; }
            else if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true) ratio = 1.6f;

            Vector3 hit_pos = collider.ClosestPoint(melee_area[0].transform.position);
            collider.GetComponent<Monster>().ReceiveDamage(unit_stats.Total_Str * ratio, hit_pos);
        }
    }
}
