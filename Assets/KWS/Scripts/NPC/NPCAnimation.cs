using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    [SerializeField]
    private string idle_motion_name;
    private Animator npc_animator;


    private void Awake()
    {
        npc_animator = GetComponent<Animator>();
        npc_animator.Play(idle_motion_name);
    }
}
