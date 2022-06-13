using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillGetKeyInput : MonoBehaviour
{
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    private Dictionary<KeyCode, SkillInfo> skill_input_key_dic = new Dictionary<KeyCode, SkillInfo>();

    private void Start()
    {
        interaction_slot_event.MountSkillSlot += AddSkillKey;
        interaction_slot_event.UnmountSkillSlot += RemoveSkillKey;
    }

    void AddSkillKey(KeyCode key_code, SkillInfo skill_info)
    {
        Debug.Log("Add Skill");

        if (skill_info == null) return;
        
        if(skill_input_key_dic.ContainsKey(key_code) == true)
        {
            Debug.LogError("Key Code Overlap");

            return;
        }

        skill_input_key_dic.Add(key_code, skill_info);
    }

    void RemoveSkillKey(KeyCode key_code)
    {
        if(skill_input_key_dic.ContainsKey(key_code) == false)
        {
            Debug.LogError("Key Code Is Not Exist");

            return;
        }

        skill_input_key_dic.Remove(key_code);
    }

    private void Update()
    {
        CheckKeyInput();
    }
    void CheckKeyInput()
    {
        if (skill_input_key_dic.Count == 0) return;

        foreach (KeyValuePair<KeyCode, SkillInfo> pair in skill_input_key_dic)
        {
            if (Input.GetKeyDown(pair.Key) == true)
            {
                DoSkill(pair.Value);

                return;
            }
        }
    }

    void DoSkill(SkillInfo skill_info)
    {
        Debug.Log("스킬발동: " + skill_info.ToString());
    }
}
