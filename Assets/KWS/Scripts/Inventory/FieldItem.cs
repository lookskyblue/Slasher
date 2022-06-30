using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField] private ItemDB item_db;
    [SerializeField] private InteractionUIEvent interaction_event;
    private Item acquired_item = null;
    void Start()
    {
        TriggerCallback trigger_callback = GetComponent<TriggerCallback>();

        trigger_callback.collision_stay_event = StayFieldItemZone;
        trigger_callback.collision_exit_event = ExitFieldItemZone;
    }

    void StayFieldItemZone(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;
        
        interaction_event.Get_Field_Item_Text_UI(true);

        if (Input.GetKey(KeyCode.E))
        {
            if (acquired_item == null) acquired_item = GetRandomItem();
            if(InventoryManager.instance.AddItem(acquired_item, acquired_item.item_cnt) == false) return;

            interaction_event.Get_Field_Item_Text_UI(false);
            interaction_event.Show_Acquired_Item_UI(acquired_item);
            
            ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("RandomItem", gameObject);
        }
    }
    void ExitFieldItemZone(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        interaction_event.Get_Field_Item_Text_UI(false);
    }

    public Item GetRandomItem()
    {
        return item_db.GetRandomItem();
    }
}
