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
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField] private int max_useable_slot_cnt;

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

    private Action<int> on_changed_gold;

    public Action<int> OnChangedGold
    {
        get { return on_changed_gold; }
        set { on_changed_gold = value; }
    }

    #endregion

    [HideInInspector] public List<Item> items = new List<Item>();

    [SerializeField] private EquipmentSlot helmet_slot;
    [SerializeField] private EquipmentSlot sword_slot;
    [SerializeField] private EquipmentSlot shield_slot;
    [SerializeField] private PotionSlot[] potion_slots;
    [SerializeField] private int slot_cnt;
    [SerializeField] private UnitStats player_stats;
    [SerializeField] private InteractionUIEvent interaction_ui_event;
    [SerializeField] private string full_item_alert_text;
    [SerializeField] private ItemDB item_db;
    private int gold_on_hand;

    public int Gold_On_Hand
    {
        get { return gold_on_hand; }
        set 
        { 
            gold_on_hand = value;
            on_changed_gold.Invoke(gold_on_hand);
        }
    }
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

        RoadGold();
    }
    void RoadGold()
    {
        on_changed_gold.Invoke(gold_on_hand);
    }

    public void ApplyAllMountedItemStats()
    {
        ApplyEachMountedItemStats(GetSlotType(ItemType.Sword));
        ApplyEachMountedItemStats(GetSlotType(ItemType.Shield));
    }

    void ApplyEachMountedItemStats(EquipmentSlot equipment_slot)
    {
        if (equipment_slot != null && equipment_slot.Is_Mount == true)
        {
            int str = 0;
            int def = 0;

            equipment_slot.GetMountedItemStats(ref str, ref def);
            player_stats.Total_Str += str;
            player_stats.Total_Def += def;
        }
    }

    bool IsFullInventory()
    {
        if (items.Count >= Slot_Cnt)
        {
            Debug.Log(full_item_alert_text);
            interaction_ui_event.On_Change_Alert_Text_UI.Invoke(full_item_alert_text);

            return true;
        }

        return false;
    }
    public bool AddItem(Item item, int item_cnt)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item_key.Equals(item.item_key) == true) // 아이템 이미 존재
            {
                if (items[i].is_stackable == true)
                {
                    items[i].item_cnt += item_cnt;
                    OnChangedItemCnt.Invoke(item.item_key);

                    return true;
                }

                else if (IsFullInventory() == true) return false;
            }

            else if (IsFullInventory() == true) return false;
        }

        item.item_cnt = item_cnt;
        items.Add(item);
        OnItemAdded.Invoke(item);

        return true;
    }

    public void AddItem(InventoryData inventory_data)
    {
        List<ItemData> item_data_list = inventory_data.item_data_list;

        for (int i = 0; i < item_data_list.Count; i++)
        {
            // ItemDB를 조회해서 아이템 고유 번호와 맞는 녀셕들을 가져온다.
            Item item = item_db.GetItem(item_data_list[i].item_id);
            item.item_cnt = item_data_list[i].item_cnt;
            
            int slot_idx = item_data_list[i].slot_idx;
            bool is_mounted = item_data_list[i].is_mounted;

            if(item == null)
            {
                Debug.LogError("Can not find item");

                continue;
            }

            items.Add(item);
            InventoryUI.instance.AddItem(item, slot_idx, is_mounted);
        }
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
        //StartCoroutine(UnmountItemCor(item_type));

        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if (equipment_slot != null && equipment_slot.Is_Mount != false)
        {
            int slot_num = equipment_slot.Slot_Num;
            equipment_slot.RemoveSlotUI();

            OnChangedMountState(slot_num);
        }
    }

    IEnumerator UnmountItemCor(ItemType item_type)
    {
        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if(equipment_slot != null && equipment_slot.Is_Mount != false)
        {
            yield return null;

            int slot_num = equipment_slot.Slot_Num;
            equipment_slot.RemoveSlotUI();

            OnChangedMountState(slot_num);
        }
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
        //StartCoroutine(MountItemCor(item_type, item, slot_num));

        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if (equipment_slot != null)
        {
            equipment_slot.UpdateSlotUI(item, slot_num);
        }
    }

    IEnumerator MountItemCor(ItemType item_type, Item item, int slot_num)
    {
        yield return null;
        EquipmentSlot equipment_slot = GetSlotType(item_type);

        if (equipment_slot != null)
        {
            yield return null;
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

    public void RemoveItem(Item item, int slot_num, int item_cnt)
    {
        RemoveItemInList(item, ref item_cnt);
        InventoryUI.instance.OnRemovedItem(slot_num, item_cnt);
    }
    private void RemoveItemInList(Item item, ref int item_cnt)
    {
        for (int i = 0; i < items.Count; i++)
        {
            //if (items[i] == item)
            if (items[i].Equals(item) == true)
            {
                items[i].item_cnt -= item_cnt;
                item_cnt = items[i].item_cnt;

                if (items[i].item_cnt <= 0)
                {
                    items.RemoveAt(i);
                }

                break;
            }
        }
    }
    private void RemoveItemInList(string item_key)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item_key.Equals(item_key) == true)
            {
                items.RemoveAt(i);
                break;
            }
        }
    }
}
