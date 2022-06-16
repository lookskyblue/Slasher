using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/InteractionUIEvent")]
public class InteractionUIEvent : ScriptableObject
{
    private Action<bool> get_field_item_text_ui;
    private Action<Item> show_acquired_item_ui;
    private Action<Item, Vector3> show_item_info_ui;
    private Action<SkillInfo, Vector3> show_skill_info_ui;
    private Action hide_item_info_ui;
    private Action hide_skill_info_ui;
    public Action<bool> Get_Field_Item_Text_UI
    {
        get { return get_field_item_text_ui; }
        set { get_field_item_text_ui = value; }
    }
    public Action<Item> Show_Acquired_Item_UI
    {
        get { return show_acquired_item_ui; }
        set { show_acquired_item_ui = value; }
    }

    public Action<Item, Vector3> Show_Item_Info_UI
    {
        get { return show_item_info_ui; }
        set { show_item_info_ui = value; }
    }

    public Action Hide_Item_Info_UI
    {
        get { return hide_item_info_ui; }
        set { hide_item_info_ui = value; }
    }

    public Action<SkillInfo, Vector3> Show_Skill_Info_UI
    {
        get { return show_skill_info_ui; }
        set { show_skill_info_ui = value; }
    }

    public Action Hide_Skill_Info_UI
    {
        get { return hide_skill_info_ui; }
        set { hide_skill_info_ui = value; }
    }
}
