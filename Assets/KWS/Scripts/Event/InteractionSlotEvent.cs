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
}
