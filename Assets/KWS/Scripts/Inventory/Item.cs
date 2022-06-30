using System;
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
    public int id;
    public string item_key;
    public int wearable_level;
    public Sprite item_image;
    public bool is_stackable;
    public int item_cnt;
    public int str;
    public int def;
    public int hp_recovery_amount;
    public int mp_recovery_amount;
    public string effect_pooling_key;
    public float cool_time;
    public string name;
    public int price;

    private bool is_cool_time = false;
    public bool Is_Cool_Time { get { return is_cool_time; } }
    private UnitStats unit_stats;
    public UnitStats Unit_Stats
    {
        get { return unit_stats; }
        set { unit_stats = value; }
    }

    private float remaining_cool_time;
    public float Remaining_Cool_Time
    {
        get { return remaining_cool_time; }
    }
    public int Use(Action sound_action)
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

        if (is_cool_time == true)
        {
            Debug.Log("In cool time");

            return 0;
        }

        DoCoolTime();

        unit_stats.AcceptUsedPotion(hp_recovery_amount, mp_recovery_amount);

        ActiveOnUsingPotionEffect();

        sound_action.Invoke();

        return DecreaseItemCnt();
    }

    void DoCoolTime()
    {
        MonoBehaviour mono = GameManager.instance.BorrowMono;

        mono.StartCoroutine(ActiveCoolTime());
    }

    IEnumerator ActiveCoolTime()
    {
        is_cool_time = true;
        remaining_cool_time = 0f;

        while(remaining_cool_time < cool_time)
        {
            remaining_cool_time += Time.deltaTime;

            yield return null;
        }
     
        is_cool_time = false;
    }

    void ActiveOnUsingPotionEffect()
    {
        if (string.IsNullOrEmpty(effect_pooling_key) == true)
        {
            Debug.LogError("Pooling key is not found");

            return;
        }

        MonoBehaviour temp_mono = GameManager.instance.BorrowMono;
        temp_mono.StartCoroutine(ShowUsingPotionEffect(effect_pooling_key));
    }

    IEnumerator ShowUsingPotionEffect(string key)
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue(effect_pooling_key);
        
        if (obj != null)
        {
            ParticleSystem using_potion_particle = obj.GetComponent<ParticleSystem>();

            using_potion_particle.transform.SetParent(ObjectPoolingManager.Instance.Player_Transform);
            using_potion_particle.transform.position = ObjectPoolingManager.Instance.Player_Transform.position;
            using_potion_particle.Play();

            float total_time = 0f;

            while(total_time <= 1f)
            {
                //using_potion_particle.transform.position = ObjectPoolingManager.Instance.Player_Transform.position;
                total_time += Time.deltaTime;
                
                yield return null;
            }

            ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue(key, obj);
        }
    }
    public Item DeepCopy()
    {
        Item new_copy = new Item();

        new_copy.item_type = this.item_type;
        new_copy.item_key = this.item_key;
        new_copy.wearable_level = this.wearable_level;
        new_copy.item_image = this.item_image;
        new_copy.is_stackable = this.is_stackable;
        new_copy.item_cnt = this.item_cnt;
        new_copy.str = this.str;
        new_copy.def = this.def;
        new_copy.hp_recovery_amount = this.hp_recovery_amount;
        new_copy.mp_recovery_amount = this.mp_recovery_amount;
        new_copy.effect_pooling_key = this.effect_pooling_key;
        new_copy.cool_time = this.cool_time;
        new_copy.is_cool_time = this.is_cool_time;
        new_copy.name = this.name;
        new_copy.price = this.price;
        new_copy.id = this.id;

        return new_copy;
    }

    int DecreaseItemCnt()
    {
        item_cnt--;

        if(item_cnt <= 0)
        {
            InventoryManager.instance.ReportItemCntIsZero(item_key);
        }

        return item_cnt;
    }
}
