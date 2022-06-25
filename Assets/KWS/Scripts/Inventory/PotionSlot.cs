using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PotionSlot : Slot
{
    [SerializeField] private InteractionSlotEvent interaction_slot_event;
    [SerializeField] private UnitStats unit_stats;
    [SerializeField] private Image cool_time_ui;
    [SerializeField] private ItemType default_type;
    [SerializeField] private KeyCode key_code;
    [SerializeField] private int potion_slot_num;
    [SerializeField] private byte color_alpha;
    private Color32 default_color;

    private void Start()
    {
        default_color = new Color32(255, 255, 255, 255);
    }
    public void RemoveSlotUI()
    {
        item.Unit_Stats = null;

        base.RemoveSlotUI();
        //item_image.gameObject.SetActive(true); // ����?
        item_cnt_text.gameObject.SetActive(false);
        cool_time_ui.gameObject.SetActive(false);

        SetDefaultColor();

        interaction_slot_event.Unmount_Potion.Invoke(key_code);
    }

    public void UpdateSlotUI(Item item, int slot_num)
    {
        base.UpdateSlotUI(item);
        Slot_Num = slot_num;
        Is_Mount = true;

        SetNormalColor();
        StartCoroutine(UpdateCoolTimeUI());

        item.Unit_Stats = unit_stats;
        interaction_slot_event.Mount_Potion.Invoke(key_code, ItemUse);
    }

    void ItemUse()
    {
        if (item.Use() == 0) return; // ������ ���

        StartCoroutine(UpdateCoolTimeUI());
        UpdateItemCnt(item.item_cnt);
        InventoryManager.instance.ReportChangedItemCntToInventorySlot(item.item_key);
    }

    IEnumerator UpdateCoolTimeUI()
    {
        if (item.Is_Cool_Time == true)
        {
            cool_time_ui.gameObject.SetActive(true);

            float remaining_cool_time = item.Remaining_Cool_Time;
            float max_cool_time = item.cool_time;

            while(remaining_cool_time < max_cool_time)
            {
                remaining_cool_time += Time.deltaTime;
                cool_time_ui.fillAmount = 1 - remaining_cool_time / max_cool_time;

                yield return null;
            }

            cool_time_ui.gameObject.SetActive(false);
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

            // ����� ���Կ� �ٸ� ������ �̹� �ִٸ� ������ �̹� �ִ� ������ �����ش�.
            if (item != null)
                InventoryManager.instance.UnmountItem(potion_slot_num);

            // ����� �������� �̹� �Һ񽽷Կ� ���� �Ѵٸ� ���� �Ѵ�.
            InventoryManager.instance.UnmountPotionByName(drag_and_drop_container.item.item_key);

            // ����� �������� ��� �Ѵ�.
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
        else return item.item_key;
    }

    public int GetPotionSlotIdx()
    {
        return potion_slot_num;
    }

    public bool IsEmptyItem() { return item == null; }
}
