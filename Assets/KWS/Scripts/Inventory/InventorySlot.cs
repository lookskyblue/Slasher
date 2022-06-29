using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : Slot
{
    public override void RemoveSlotUI()
    {
        //StartCoroutine(RemoveSlotUICor());

        base.RemoveSlotUI();

        item_cnt_text.text = null;
        item_cnt_text.gameObject.SetActive(false);
        item_mount_state_text.gameObject.SetActive(false);
    }
    IEnumerator RemoveSlotUICor()
    {
        base.RemoveSlotUI();

        yield return null;

        item_cnt_text.text = null;
        item_cnt_text.gameObject.SetActive(false);
        item_mount_state_text.gameObject.SetActive(false);
    }
    public override void UpdateSlotUI(Item item)
    {
        base.UpdateSlotUI(item);
    }

    public void Unmount()
    {
        item_mount_state_text.gameObject.SetActive(false);
        Is_Mount = false;
    }

    public ItemType GetItemType()
    {
        return item.item_type;
    }
    public Item GetItem()
    {
        return item;
    }

    public void SetNativeSize()
    {
        if (item != null)
            item_image.SetNativeSize();
    }
}
