using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionSlot : Slot
{
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    [SerializeField]
    private ItemType default_type;
    [SerializeField]
    private int potion_slot_num;
    [SerializeField]
    private byte color_alpha;
    private Color32 default_color;

    [SerializeField]
    private KeyCode key_code;

    [SerializeField]
    private UnitStats unit_stats;
    private void Start()
    {
        default_color = new Color32(255, 255, 255, 255);
    }
    public void RemoveSlotUI()
    {
        item.Unit_Stats = null;

        base.RemoveSlotUI();
        //item_image.gameObject.SetActive(true); // 이유?
        item_cnt_text.gameObject.SetActive(false);

        SetDefaultColor();

        interaction_slot_event.Unmount_Potion.Invoke(key_code);
    }

    public void UpdateSlotUI(Item item, int slot_num)
    {
        base.UpdateSlotUI(item);
        Slot_Num = slot_num;
        Is_Mount = true;

        SetNormalColor();

        item.Unit_Stats = unit_stats;

        interaction_slot_event.Mount_Potion.Invoke(key_code, ItemUse);
    }

    void ItemUse()
    {
        if(item.Use() != 0)
        {
            UpdateItemCnt(item.item_cnt);
            InventoryManager.instance.ReportChangedItemCntToInventorySlot(item.item_name);
        }
    }

    public override void UpdateItemCnt(int cnt)
    {
        item_cnt_text.text = cnt.ToString();
    }

    void SetDefaultColor()
    {
        default_color.a = color_alpha;
        item_image.color = default_color;
    }

    void SetNormalColor()
    {
        default_color.a = 255;
        item_image.color = default_color;
    }
    public override void OnPointerUp(PointerEventData pointer_event_data)
    {
        if (pointer_event_data.button != PointerEventData.InputButton.Right) return;
        if (item == null) return;

        InventoryManager.instance.UnmountItem(potion_slot_num);
    }
    public override void OnDrop(PointerEventData event_data)
    {
        if (drag_and_drop_container.item != null)
        {
            if (default_type != drag_and_drop_container.item.item_type)
                return;

            // 드랍된 슬롯에 다른 포션이 이미 있다면 기존에 이미 있는 포션은 지워준다.
            if (item != null)
                InventoryManager.instance.UnmountItem(potion_slot_num);

            // 드랍된 아이템이 이미 소비슬롯에 존재 한다면 해제 한다.
            InventoryManager.instance.UnmountPotionByName(drag_and_drop_container.item.item_name);

            // 드랍된 아이템을 등록 한다.
            Slot_Num = drag_and_drop_container.slot_num;
            UpdateSlotUI(drag_and_drop_container.item, Slot_Num);
            drag_and_drop_container.is_mount = true;
        }
    }
    public override void OnBeginDrag(PointerEventData event_data)
    {
        drag_and_drop_container.item = null;
    }

    public override void OnDrag(PointerEventData event_data)
    {
    }

    public override void OnEndDrag(PointerEventData event_data)
    {
    }

    public string GetPotionName()
    {
        if (item == null) return string.Empty;
        else return item.item_name;
    }

    public int GetPotionSlotIdx()
    {
        return potion_slot_num;
    }

    public bool IsEmptyItem() { return item == null; }
}
