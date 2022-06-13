using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private float fire_gap_time;
    private Animator player_animator;
    private int attack_click_cnt = 0;
    private float last_clicked_time = 0f;
    private float max_combo_delay = 0.5f;
    private PlayerMovement player_movement;
    [SerializeField]
    private UIActiveGetKeyInput ui_active_get_key_input;
    private bool is_mouse_inside_in_skill_slot = false;
    private void Awake()
    {
        player_animator = GetComponent<Animator>();
        player_movement = GetComponent<PlayerMovement>();
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
        if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true)
        {
            SetAttackPhaseToZero();
        }

        if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true)
        {
            SetAttackPhaseToZero();
            attack_click_cnt = 0;
        }

        if (Time.time - last_clicked_time > max_combo_delay)
        {
            attack_click_cnt = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (ui_active_get_key_input.CheckClickOutOfUIs(Input.mousePosition) == false) return;
            if (is_mouse_inside_in_skill_slot == true) return;

            else // UI 밖 클릭
            {
                //1. UI가 하나라도 켜져있을 경우 UI다 끄고 리턴.
                if (ui_active_get_key_input.ActiveOffAllToggleObj() == true)
                {
                    return;
                }
            }

            //2. UI가 아무것도 안 켜져있을 경우 어택.
            Attack();
        }
    }
    public void InitAttackAnimation()
    {
        SetAttackPhaseToZero();
        attack_click_cnt = 0;
    }

    void Attack()
    {
        last_clicked_time = Time.time;
        attack_click_cnt++;

        if (attack_click_cnt == 1)
        {
            //player_animator.SetInteger("attack_phase", 0);
            player_animator.SetInteger("attack_phase", 1);
        }

        attack_click_cnt = Mathf.Clamp(attack_click_cnt, 0, 2);

        if (attack_click_cnt >= 2)
        {
            if (player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0"))
            {
                player_animator.SetInteger("attack_phase", 2);
                player_movement.CheckTurnTiming();
                attack_click_cnt = 0;
            }

            else if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == false)
                attack_click_cnt = 0;
        }
    }

    public void SetAttackPhaseToZero() 
    {
        player_animator.SetInteger("attack_phase", 0); 
    }
}

