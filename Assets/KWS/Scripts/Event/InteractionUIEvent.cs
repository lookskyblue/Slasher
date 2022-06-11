using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/InteractionUIEvent")]
public class InteractionUIEvent : ScriptableObject
{
    private Action<bool> get_field_item_text_ui;
    private Action<Item> show_acquired_item_ui;

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
}
