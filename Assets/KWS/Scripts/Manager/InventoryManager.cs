using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField]
    private int max_useable_slot_cnt;

    #region 대리자 정의
    private Action<int> on_slot_cnt_change;
    public Action<int> OnSlotCntChange 
    {
        set { on_slot_cnt_change = value; } 
        get { return on_slot_cnt_change; }
    }

    private Action<Item> on_item_added;
    public Action<Item> OnItemAdded 
    {
        set { on_item_added = value; } 
        get { return on_item_added; }
    }

    private Action<string> on_changed_item_cnt;
    public Action<string> OnChangedItemCnt
    {
        set { on_changed_item_cnt = value; }
        get { return on_changed_item_cnt; }
    }

    private Action<int> on_changed_mount_state;
    public Action<int> OnChangedMountState
    {
        set { on_changed_mount_state = value; }
        get { return on_changed_mount_state; }
    }

    private Action<int> on_removed_item;
    public Action<int> OnRemovedItem
    {
        get { return on_removed_item; }
        set { on_removed_item = value; }
    }

    #endregion

    [HideInInspector]
    public List<Item> items = new List<Item>();

    [SerializeField]
    private EquipmentSlot helmet_slot;
    [SerializeField]
    private EquipmentSlot sword_slot;
    [SerializeField]
    private EquipmentSlot shield_slot;
    [SerializeField]
    private PotionSlot[] potion_slots;

    private int slot_cnt;
    public int Slot_Cnt
    {
        get => slot_cnt;
        set
        {
            slot_cnt = value;
            OnSlotCntChange.Invoke(slot_cnt);
        }
    }
    private void Start()
    {
        Slot_Cnt = max_useable_slot_cnt;
        //slot_cnt = max_useable_slot_cnt;
    }
    public bool AddItem(Item item)
    {
        if (items.Count >= Slot_Cnt) return false;

        //Debug.Log("획득한 아이템명: " + item.item_name);

        if (item.is_stackable == false)
        {
            items.Add(item);
        }

        else // 리스트 순회 검사
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item_name.Equals(item.item_name) == true &&
                    items[i].is_stackable == true) // 같은 아이템
                {
                    items[i].item_cnt++;

                    OnChangedItemCnt.Invoke(item.item_name);
                    return true;
                }
            }

            items.Add(item);

        }  // UI에도 개수 표기 할 수 있게 작업할 것

        OnItemAdded.Invoke(item);
        return true;
    }

    public void ReportSlotNumToEquipmentSlotNum(ItemType item_type, int new_slot_num) // 중요
    {
        EquipmentSlot equipment_slot = GetSlotType(item_type);

        equipment_slot.Slot_Num = new_slot_num;
    }

    public void ReportSlotNumToPotionSlotNum(string potion_name, int new_slot_num)
    {
        for(int i = 0; i < potion_slots.Length; i++)
        {
            if(potion_slots[i].GetPotionName().Equals(potion_name) == true)
            {
                potion_slots[i].Slot_Num = new_slot_num;
                break;
            }
        }
    }

    public void UnmountItem(ItemType item_type) // 중요
    {
        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if (equipment_slot == null) return;
        if (equipment_slot.Is_Mount == false) return;
        
        int slot_num = equipment_slot.Slot_Num;
        equipment_slot.RemoveSlotUI();

        Debug.Log("4.1: 장비창의 장착 해제 아이템 번호: " + slot_num);

        OnChangedMountState(slot_num);
    }
    public void UnmountItem(int potion_slot_idx) // 중요
    {
        PotionSlot potion_slot = potion_slots[potion_slot_idx];
        int slot_num = potion_slot.Slot_Num;
        potion_slot.RemoveSlotUI();

        OnChangedMountState(slot_num);
    }

    public void MountItem(ItemType item_type, Item item, int slot_num) // 중요
    {
        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if (equipment_slot != null)
        {
            equipment_slot.UpdateSlotUI(item, slot_num);
        }
    }
    public void MountItem(Item potion_item, int slot_num)
    {
        for(int i  = 0; i < potion_slots.Length; i++)
        {
            if(potion_slots[i].IsEmptyItem() == true)
            {
                potion_slots[i].UpdateSlotUI(potion_item, slot_num);
                return;
            }
        }

        // 빈 공간이 없다면 처음 슬롯을 비워서 넣자.

        UnmountItem(potion_slots[0].GetPotionSlotIdx());

        potion_slots[0].UpdateSlotUI(potion_item, slot_num);
    }
    private EquipmentSlot GetSlotType(ItemType item_type) // 중요
    {
        if (item_type == ItemType.Sword) return sword_slot;
        else if (item_type == ItemType.Shield) return shield_slot;
        //else if(item_type == ItemType.Potion) return 

        else return null;
    }

    public void UnmountPotionByName(string potion_name)
    {
        for(int i = 0; i < potion_slots.Length; i++)
        {
            if(potion_slots[i].GetPotionName().Equals(potion_name) == true)
            {
                potion_slots[i].RemoveSlotUI();
                break;
            }
        }
    }    
    public void ReportChangedItemCntToPotionSlot(string potion_name, int potion_cnt)
    {
        for(int i = 0; i < potion_slots.Length; i++)
        {
            if(potion_slots[i].GetPotionName().Equals(potion_name) == true)
            {
                potion_slots[i].UpdateItemCnt(potion_cnt);
                break;
            }
        }
    }
    public void ReportChangedItemCntToInventorySlot(string item_name)
    {
        OnChangedItemCnt(item_name);
    }
    public void ReportItemCntIsZero(string potion_name)
    {
        for (int i = 0; i < potion_slots.Length; i++)
        {
            if (potion_slots[i].GetPotionName().Equals(potion_name) == true)
            {
                int slot_num = potion_slots[i].Slot_Num;
                
                potion_slots[i].RemoveSlotUI();
                InventoryManager.instance.OnRemovedItem.Invoke(slot_num);

                RemoveItemInList(potion_name);

                break;
            }
        }
    }
    private void RemoveItemInList(string item_name)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item_name.Equals(item_name) == true)
            {
                items.RemoveAt(i);

                break;
            }
        }
    }
}
