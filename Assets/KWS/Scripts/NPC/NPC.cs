using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string idle_motion_name;
    private Animator npc_animator;

    private void Awake()
    {
        InitTriggerCallback();
        InitAnimation();
    }

    void InitTriggerCallback()
    {
        GetComponent<TriggerCallback>().collision_enter_event = CheckEnterUser;
        GetComponent<TriggerCallback>().collision_exit_event = CheckExitUser;
    }

    void InitAnimation()
    {
        npc_animator = GetComponent<Animator>();
        npc_animator.Play(idle_motion_name);
    }
    void CheckEnterUser(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        ShowUI();
    }
    void CheckExitUser(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        HideUI();
    }
    protected virtual void ShowUI()
    {
    }

    protected virtual void HideUI()
    {
    }
}
