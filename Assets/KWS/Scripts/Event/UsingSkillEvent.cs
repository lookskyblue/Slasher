using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/UsingSkillEvent")]

public class UsingSkillEvent : ScriptableObject
{
    private Action<bool> on_change_skill_state;
    public Action<bool> OnChangeSkillState
    {
        get { return on_change_skill_state; }
        set { on_change_skill_state = value; }
    }
}
