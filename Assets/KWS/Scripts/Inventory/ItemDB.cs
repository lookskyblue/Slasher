using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField] private List<Item> item_db;
    public List<Item> Item_DB { get { return item_db; } }

    public Item GetRandomItem()
    {
        int rand_idx = Random.Range(0, item_db.Count);
        return item_db[rand_idx].DeepCopy();
    }

    public List<Item> GetItemList(ItemType item_type)
    {
        List<Item> item_list = new List<Item>();
        int item_db_count = item_db.Count;

        for(int i = 0; i < item_db_count; i++)
        {
            if(item_db[i].item_type == item_type)
            {
                item_list.Add(item_db[i].DeepCopy());
            }
        }

        return item_list;
    }

    public Item GetItem(int id)
    {
        for(int i = 0; i < item_db.Count; i++)
        {
            if (item_db[i].id == id) return item_db[i].DeepCopy();
        }

        return null;
    }
}
