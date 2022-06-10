using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField]
    private List<Item> item_db;

    public Item GetRandomItem()
    {
        int rand_idx = Random.Range(0, item_db.Count);
        return item_db[rand_idx].DeepCopy();
    }
}
