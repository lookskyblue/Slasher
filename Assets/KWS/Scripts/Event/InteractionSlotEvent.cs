using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/InteractionSlotEvent")]

public class InteractionSlotEvent : ScriptableObject
{
    private Action<string, int> mount_sword;
    private Action<string, int> mount_shield;
    private Action<KeyCode, Action> mount_potion;
    private Action<int> unmount_sword;
    private Action<int> unmount_shield;
    private Action<KeyCode> unmount_potion;
    private Action<float> show_cool_time_ui;
    private Action<bool> using_skill_slot;
    private Action<KeyCode, SkillInfo> mount_skill_slot;
    private Action<KeyCode> unmount_skill_slot;

    public Action<string, int> Mount_Shield
    {
        get { return mount_shield; }
        set { mount_shield = value; }
    }
    public Action<string, int> Mount_Sword
    {
        get { return mount_sword; }
        set { mount_sword = value; }
    }
    public Action<int> Unmount_Sword
    {
        get { return unmount_sword; }
        set { unmount_sword = value; }
    }

    public Action<int> Unmount_Shield
    {
        get { return unmount_shield; }
        set { unmount_shield = value; }
    }

    public Action<KeyCode, Action> Mount_Potion
    {
        get { return mount_potion; }
        set { mount_potion = value; }
    }

    public Action<KeyCode> Unmount_Potion
    {
        get { return unmount_potion; }
        set { unmount_potion = value; }
    }
    public Action<float> ShowCoolTimeUI
    {
        get { return show_cool_time_ui; }
        set { show_cool_time_ui = value; }
    }

    public Action<bool> UsingSkillSlot
    {
        get { return using_skill_slot; }
        set { using_skill_slot = value; }
    }

    public Action<KeyCode, SkillInfo> MountSkillSlot
    {
        get { return mount_skill_slot; }
        set { mount_skill_slot = value; }
    }

    public Action<KeyCode> UnmountSkillSlot
    {
        get { return unmount_skill_slot; }
        set { unmount_skill_slot = value; }
    }
}
