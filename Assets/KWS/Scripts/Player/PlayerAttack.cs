using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [System.Serializable]
    public struct AudioClipInfo
    {
        public string name;
        public AudioClip audio_clip;
    }
    [SerializeField ] private UIActiveGetKeyInput ui_active_get_key_input;
    [SerializeField] private float fire_gap_time;
    [SerializeField] private AudioClipInfo[] audio_clip_info;
    private Dictionary<string, AudioClip> sound_dic = new Dictionary<string, AudioClip>();
    private AudioSource audio_source;

    private PlayerMovement player_movement;
    private Animator player_animator;
    private float last_clicked_time = 0f;
    private float max_combo_delay = 0.7f;
    private int attack_click_cnt = 0;
    private bool is_mouse_inside_in_skill_slot = false;
    private bool is_doing_attack_stop_cor;

    public bool Is_Doing_Attack_Stop_Cor { get { return is_doing_attack_stop_cor; } }
    private void Awake()
    {
        audio_source = GetComponent<AudioSource>();
        player_animator = GetComponent<Animator>();
        player_movement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        for(int i = 0; i < audio_clip_info.Length; i++)
        {
            sound_dic.Add(audio_clip_info[i].name, audio_clip_info[i].audio_clip);
        }
    }

    public void IsMouseInsideInSkillSlots(bool value)
    {
        is_mouse_inside_in_skill_slot = value;
    }
    private void Update()
    {
        AnimationCheck();
    }

    void AnimationCheck()
    {
        if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.2f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true)
            player_animator.SetInteger("attack_phase", 0);

        if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true)
        {
            player_animator.SetInteger("attack_phase", 0);
            player_animator.SetInteger("attack_to_idle", 1);
        }

        if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true)
        {
            //SetAttackPhaseToZero();
        }

        //if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true)
        //{
        //    SetAttackPhaseToZero();
        //    attack_click_cnt = 0;

        //    ResetAttackPhase();
        //}

        if (Time.time - last_clicked_time > max_combo_delay)
        {
            //attack_click_cnt = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (ui_active_get_key_input.CheckClickOutOfUIs(Input.mousePosition) == false) return;
            if (is_mouse_inside_in_skill_slot == true) return;
            if (IsDoingAnotherAnimation() == true) return;
            if (GameManager.instance.IsDoingOtherTask() == true) return;
            else // UI 밖 클릭
            {
                //1. UI가 하나라도 켜져있을 경우 UI다 끄고 리턴.
                if (ui_active_get_key_input.ActiveOffAllToggleObj() == true)
                {
                    return;
                }
            }

            //2. UI가 아무것도 안 켜져있을 경우 어택.
            //Attack();
            Attack2();
        }
    }
    public bool IsDoingAnotherAnimation()
    {
        if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("GroundCrack") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Smash") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("CannonShot") == true) 
            return true;

        return false;
    }
    void Attack()
    {
        last_clicked_time = Time.time;
        attack_click_cnt++;

        if (attack_click_cnt == 1)
        {
            player_animator.SetInteger("attack_phase", 1);
        }

        if (attack_click_cnt >= 2)
        {
            if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0"))
            {
                player_animator.SetInteger("attack_phase", 2);
                player_movement.CheckTurnTiming();
            }

            else if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == false)
                ;// attack_click_cnt = 0;
        }

        if (attack_click_cnt >= 3)
        {
            if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
            {
                player_animator.SetInteger("attack_phase", 3);
                player_movement.CheckTurnTiming();
                attack_click_cnt = 0;
            }

            else if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == false)
                attack_click_cnt = 0;
        }

        attack_click_cnt = Mathf.Clamp(attack_click_cnt, 0, 3);
    }

    void Attack2()
    {
        //if(is_attack_phase_reset == true)
        //if (is_cor == true ||
        if (
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("GroundCrack") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Smash") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("CannonShot") == true)
            return;

        player_animator.SetInteger("attack_phase", 1);

        //       is_attack_phase_reset = false;
    }
    IEnumerator AllowNextAttack(int next_attack_phase)
    {
        float total_time = 0f;

        while(total_time < 0.7f)
        {
            total_time += Time.deltaTime;

            if(Input.GetMouseButtonDown(0) == true)
            {
                player_animator.SetInteger("attack_phase", next_attack_phase);
                player_movement.CheckTurnTiming();

                yield break;
            }

            yield return null;
        }
    }

    IEnumerator StopNowAttackAnimation(int now_attack_phase)
    {
        is_doing_attack_stop_cor = true;
        
        player_animator.SetInteger("attack_phase", 0);
        //player_animator.SetInteger("attack_to_idle", now_attack_phase);

        while (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack" + (now_attack_phase - 1)) == true)
        {
            player_animator.SetInteger("attack_to_idle", now_attack_phase);
            yield return null;
        }

        yield return null;
        player_animator.SetInteger("attack_to_idle", 0);

        is_doing_attack_stop_cor = false;
    }

    void ResetAttackPhase() 
    {
        player_animator.SetInteger("attack_phase", 0);
    }

    public void SetAttackPhaseToZero() 
    {
        player_animator.SetInteger("attack_phase", 0); 
    }

    public void StopAndAttackReset()
    {
        StopAllCoroutines();
        //SetAttackPhaseToZero();
        //player_animator.SetInteger("attack_to_idle", 2);
    }

    public IEnumerator ResetAttackAndIdlePhase()
    {
        //ResetAttackPhase();
        //player_animator.SetInteger("attack_to_idle", 0);

        for (int i = 1; i < 4; i++)
        {
            //while (is_doing_attack_stop_cor == true)
            {
                StartCoroutine(StopNowAttackAnimation(i));
                yield return null;
            }
        }
    }

    void PlaySound(string sound_name)
    {
        if(sound_dic.ContainsKey(sound_name) == false)
        {
            Debug.LogError("Does not contain sound name: " + sound_name);

            return;
        }

        audio_source.PlayOneShot(sound_dic[sound_name]);
    }
}

