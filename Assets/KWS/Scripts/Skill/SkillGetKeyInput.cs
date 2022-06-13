using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillGetKeyInput : MonoBehaviour
{
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    [SerializeField]
    private UnitStats player_stats;
    private Dictionary<KeyCode, SkillInfo> skill_input_key_dic = new Dictionary<KeyCode, SkillInfo>();
    private Animator player_ani;
    private SkillInfo now_doing_skill = null;
    private UsingSkillEvent using_skill_event;
    private void Start()
    {
        player_ani = GetComponent<Animator>();
        interaction_slot_event.MountSkillSlot += AddSkillKey;
        interaction_slot_event.UnmountSkillSlot += RemoveSkillKey;
    }

    void AddSkillKey(KeyCode key_code, SkillInfo skill_info)
    {
        Debug.Log("Add Skill");

        if (skill_info == null) return;
        
        if(skill_input_key_dic.ContainsKey(key_code) == true)
        {
            Debug.LogError("Key Code Overlap");

            return;
        }

        skill_input_key_dic.Add(key_code, skill_info);

        AddHitAreaCallback(skill_info.hit_area);
    }

    void AddHitAreaCallback(Collider hit_area)
    {
        if (hit_area == null) return;

        hit_area.GetComponent<TriggerCallback>().collision_enter_event = Hit;
    }

    void Hit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Monster") == true) // 몬스터 태그
        {
            if(now_doing_skill == null)
            {
                Debug.LogError("히트지만 아쉽게도 스킬 정보가 사라짐");

                return;
            }

            Vector3 hit_pos = collider.ClosestPoint(now_doing_skill.hit_area.transform.position);
            collider.GetComponent<Monster>().ReceiveDamage(player_stats.Total_Str * now_doing_skill.damage_ratio, hit_pos);
        }
    }

    void RemoveSkillKey(KeyCode key_code)
    {
        if(skill_input_key_dic.ContainsKey(key_code) == false)
        {
            Debug.LogError("Key Code Is Not Exist");

            return;
        }

        skill_input_key_dic.Remove(key_code);
    }

    private void Update()
    {
        CheckKeyInput();
    }
    void CheckKeyInput()
    {
        if (skill_input_key_dic.Count == 0) return;

        foreach (KeyValuePair<KeyCode, SkillInfo> pair in skill_input_key_dic)
        {
            if (Input.GetKeyDown(pair.Key) == true)
            {
                DoSkill(pair.Value);

                return;
            }
        }
    }
    void DoSkill(SkillInfo skill_info)
    {
        if (now_doing_skill != null)
        {
            Debug.Log("Still Skill Active");

            return;
        }

        now_doing_skill = skill_info;

        Debug.Log("Activate Skill");

        // 0. 발동 가능 조건인지 체크
        // ex. 마을인지, 다른 스킬 시전중인지, mp 안 부족한지, 피격중인지

        PlaySkillAnimation(skill_info.animation_key);

        ConsumeMp();
        ActiveCoolTime();
    }

    void ConsumeMp()
    {

    }
    void ActiveCoolTime()
    {

    }

    void PlaySkillAnimation(string animation_key)
    {
        Debug.Log("애니메이션 시작");

        player_ani.SetBool(animation_key, true);
    }

    IEnumerator ShowSkillParticle()
    {
        Debug.Log("파티클 시작");

        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue(now_doing_skill.particle_key);
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();

        particle.transform.position = ObjectPoolingManager.Instance.Player_Transform.position;
        particle.transform.rotation = ObjectPoolingManager.Instance.Player_Transform.rotation;
        particle.Play();

        string particle_key = now_doing_skill.particle_key;

        yield return new WaitForSeconds(now_doing_skill.during_particle_time);

        HideSkillParticle(obj, particle_key);
    }
    void EnableHitArea()
    {
        Debug.Log("히트 영역 켬");

        now_doing_skill.hit_area.enabled = true;
        //now_doing_skill.hit_area.gameObject.SetActive(true);
    }
    void DisableHitArea()
    {
        Debug.Log("히트 영역 끔");

        now_doing_skill.hit_area.enabled = false;
        //now_doing_skill.hit_area.gameObject.SetActive(false);
    }
    void HideSkillParticle(GameObject obj, string particle_key)
    {
        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue(particle_key, obj);

        Debug.Log("파티클 종료");
    }

    IEnumerator StopSkillAnimation()
    {
        Debug.Log("애니메이션 종료");

        player_ani.SetBool(now_doing_skill.animation_key, false);

        yield return new WaitForSeconds(0.5f);

        now_doing_skill = null;
    }
}
