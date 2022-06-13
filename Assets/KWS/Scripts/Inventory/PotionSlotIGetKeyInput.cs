using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSlotIGetKeyInput : MonoBehaviour
{
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    private Dictionary<KeyCode, Action> input_key_dic = new Dictionary<KeyCode, Action>();
    private void Start()
    {
        interaction_slot_event.Mount_Potion += AddPotionKey;
        interaction_slot_event.Unmount_Potion += RemovePotionKey;
    }

    private void AddPotionKey(KeyCode key_code, Action action)
    {
        if(input_key_dic.ContainsKey(key_code) == true)
        {
            Debug.LogError("Key Code Overlap"); 
            
            return;
        }

        input_key_dic.Add(key_code, action);
    }

    private void RemovePotionKey(KeyCode key_code)
    {
        if(input_key_dic.ContainsKey(key_code) == false)
        {
            Debug.LogError("Key Code Is Not Exist"); 
            
            return;
        }

        input_key_dic.Remove(key_code);
    }

    private void Update()
    {
        CheckKeyInput();
    }

    void CheckKeyInput()
    {
        if (input_key_dic.Count == 0) return;

        foreach (KeyValuePair<KeyCode, Action> pair in input_key_dic)
        {
            if (Input.GetKeyDown(pair.Key) == true)
            {
                pair.Value();
                return;
            }
        }
    }
}
