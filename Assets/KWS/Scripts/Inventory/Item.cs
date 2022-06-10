using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Sword,
    Shield,
    Helmet,
    Potion,
    Etc
}
[System.Serializable]
public class Item
{
    public ItemType item_type;
    public string item_name;
    public Sprite item_image;
    public bool is_stackable;
    public int item_cnt;
    public int str;
    public int def;
    public int hp_recovery_amount;
    
    private UnitStats unit_stats;
    public UnitStats Unit_Stats
    {
        get { return unit_stats; }
        set { unit_stats = value; }
    }
    public int Use()
    {
        if (item_type != ItemType.Potion)
        {
            Debug.Log("Not a potion type"); 
            
            return 0;
        }

        if(unit_stats == null)
        {
            Debug.LogError("Unit_stats is null");

            return 0;
        }

        Debug.Log("È¸º¹·®: " + hp_recovery_amount);

        unit_stats.AcceptUsedPotion(hp_recovery_amount);
        
        return DecreaseItemCnt();
    }

    public Item DeepCopy()
    {
        Item new_copy = new Item();

        new_copy.item_type = this.item_type;
        new_copy.item_name = this.item_name;
        new_copy.item_image = this.item_image;
        new_copy.is_stackable = this.is_stackable;
        new_copy.item_cnt = this.item_cnt;
        new_copy.str = this.str;
        new_copy.def = this.def;
        new_copy.hp_recovery_amount = this.hp_recovery_amount;

        return new_copy;
    }

    int DecreaseItemCnt()
    {
        item_cnt--;

        if(item_cnt <= 0)
        {
            InventoryManager.instance.ReportItemCntIsZero(item_name);
        }

        return item_cnt;
    }
}
