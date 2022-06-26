using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [SerializeField] private GameObject inventory_panel;
    [SerializeField] private Transform slot_holder;
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private GameObject item_info_ui;
    [SerializeField] private Text gold_text;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InventoryManager.instance.OnSlotCntChange += OnSlotCntChange;
        InventoryManager.instance.OnItemAdded += OnDrawNewItemUI;
        InventoryManager.instance.OnChangedItemCnt += OnRedrawItemCntUI;
        InventoryManager.instance.OnChangedMountState += OnChangeMountStateUI;
        InventoryManager.instance.OnRemovedItem += OnRemovedItem;
        InventoryManager.instance.OnChangedGold += OnChangedGold;

        slots = slot_holder.GetComponentsInChildren<InventorySlot>();
    }
    void OnSlotCntChange(int value)
    {
        for(int i  = 0; i < slots.Length; i++)
        {
            slots[i].Slot_Num = i;

            if (i < InventoryManager.instance.Slot_Cnt)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }

    void OnDrawNewItemUI(Item item)
    {
        for (int i = 0; i < slots.Length; i++) // 슬롯 순회해서 빈자리가 있으면 넣어
        {
            if (slots[i].GetItem() == null)
            {
                slots[i].UpdateSlotUI(item);
                slots[i].SetNativeSize();

                return;
            }
        }
    }
    void OnRedrawItemCntUI(string item_key) // 슬롯 순회 해서 슬롯의 아이템명이 같으면 업데이트
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Item item = slots[i].GetItem();
            
            if (item != null)
            {
                if (item.item_key.Equals(item_key) == true)
                {
                    slots[i].UpdateItemCnt(item.item_cnt);
                    return;
                }
            }
        }
    }

    void OnChangeMountStateUI(int slot_num)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].Slot_Num == slot_num)
            {
                slots[i].Unmount();
                //Destroy(slots[i].gameObject);
                break;
            }
        }
    }
    public bool IsActiveInventoryUI() { return inventory_panel.activeSelf; }
    public void OnRemovedItem(int slot_num)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Slot_Num == slot_num)
            {
                slots[i].RemoveSlotUI();

                break;
            }
        }
    }
    public void OnRemovedItem(int slot_num, int item_cnt)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Item item = slots[i].GetItem();
            
            if (item != null && slots[i].Slot_Num == slot_num)
            {
                item.item_cnt = item_cnt;

                if (item.item_cnt <= 0) slots[i].RemoveSlotUI();
                else slots[i].UpdateItemCnt(item.item_cnt);

                break;
            }
        }
    }
    void OnChangedGold(int gold)
    {
        if (gold == 0) gold_text.text = "0";
        else gold_text.text = GetThousandCommaText(gold);
    }
    string GetThousandCommaText(int gold)
    {
        return string.Format("{0:#,###}", gold);
    }

    //bool IsMountedItem(string item_name)
    //{
    //    for(int i = 0; i < slots.Length; i++)
    //    {
    //        if(slots[i].name.Equals(item_name) == true)
    //        {
                
    //        }
    //    }
    //}

    public void OnInventoryUI() { inventory_panel.SetActive(true); }
}
