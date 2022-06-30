using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWSWeapon;

public class PlayerWeapon : Weapon
{
    [SerializeField] private TriggerCallback trigger_callback;
    [SerializeField] private AudioClip[] hit_sound;
    private AudioSource audio_source;
    private Coroutine is_delaying_hit_sound = null;
    void Awake()
    {
        audio_source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        trigger_callback.collision_enter_event = MeleeAttacked;
    }

    public void MeleeAttacked(Collider collider)
    {
        if (collider.gameObject.CompareTag("Monster") == true) // 몬스터 태그
        {
            float ratio = 1f;

            if(unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true)
            {
                if (is_delaying_hit_sound == null) is_delaying_hit_sound = StartCoroutine(PlayHitSound(0));
            }

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true)
            {
                ratio = 1.3f;
                if (is_delaying_hit_sound == null) is_delaying_hit_sound = StartCoroutine(PlayHitSound(1));
            }

            else if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true)
            {
                ratio = 1.6f;
                if (is_delaying_hit_sound == null) is_delaying_hit_sound = StartCoroutine(PlayHitSound(2));
            }

            Vector3 hit_pos = collider.ClosestPoint(melee_area[0].transform.position);
            collider.GetComponent<Monster>().ReceiveDamage(unit_stats.Total_Str * ratio, hit_pos);
        }
    }

    IEnumerator PlayHitSound(int sound_idx)
    {
        audio_source.PlayOneShot(hit_sound[sound_idx]);
        
        yield return new WaitForSeconds(0.3f);

        is_delaying_hit_sound = null;
    }
}
