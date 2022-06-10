using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    [SerializeField]
    private Sprite defalut_equipment_image;
    [SerializeField]
    private ItemType default_type;
    [SerializeField]
    private byte color_alpha;
    private Color32 default_color;

    private void Start()
    {
        default_color = new Color32(255, 255, 255, 255);
    }
    public void RemoveSlotUI()
    {
        CallbackUnmountEquipment(item);

        base.RemoveSlotUI();

        item_image.sprite = defalut_equipment_image;
        item_image.SetNativeSize();
        item_image.gameObject.SetActive(true);

        SetDefaultColor();
    }

    public void UpdateSlotUI(Item item, int slot_num)
    {
        base.UpdateSlotUI(item);

        this.item.str = item.str;
        this.item.def = item.def;

        Slot_Num = slot_num;
        Is_Mount = true;

        SetNormalColor();
        CallbackMountEquipment(item);
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

        InventoryManager.instance.UnmountItem(item.item_type);
    }
    private void CallbackMountEquipment(Item item)
    {
        if (item.item_type == ItemType.Sword)
            interaction_slot_event.Mount_Sword(item.item_name, item.str);

        else if (item.item_type == ItemType.Shield)
            interaction_slot_event.Mount_Shield(item.item_name, item.def);
    }

    private void CallbackUnmountEquipment(Item item)
    {
        if (item.item_type == ItemType.Sword)
            interaction_slot_event.Unmount_Sword(item.str);

        else if (item.item_type == ItemType.Shield)
            interaction_slot_event.Unmount_Shield(item.def);
    }

    public override void OnDrop(PointerEventData event_data)
    {
        //base.OnDrop(event_data);
        
        if (drag_and_drop_container.item != null)
        {
            if (default_type != drag_and_drop_container.item.item_type)
                return;

            if (item != null)
            {
                InventoryManager.instance.UnmountItem(item.item_type);
            }

            Slot_Num = drag_and_drop_container.slot_num;
            UpdateSlotUI(drag_and_drop_container.item, Slot_Num);
            drag_and_drop_container.is_mount = true;
            Debug.Log("노멀 컬러");

            SetNormalColor();
        }

        //else
        //{
        //    drag_and_drop_container.item = null;
        //    drag_and_drop_container.item_image.sprite = null;
        //    drag_and_drop_container.is_mount = false;
        //    drag_and_drop_container.slot_num = -1;

        //    Debug.Log("디폴트 컬러");
            
        //    SetDefaultColor();
        //}
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
}
