using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreOwnerNPC : NPC
{
    [SerializeField] private StoreUI store_ui;
    sealed protected override void ShowUI()
    {
        store_ui.OpenStore();
    }
    sealed protected override void HideUI()
    {
        store_ui.PushCloseStore();
    }
}
