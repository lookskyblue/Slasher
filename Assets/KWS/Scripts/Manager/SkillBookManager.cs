using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class SkillInfo
{
    public Sprite icon;
    public string name;
    public int possible_level;
    public int taken_point;
    public int limit_point;
    public int mp_cost;
    public float cool_time;
    public string animation_key;
    public string particle_key;
    public float during_particle_time;
    public float damage_ratio;
    public Collider hit_area;
    public Transform spawn_transform;
    public string explain;
    private Action on_use_skill_callback;
    public Action On_Use_Skill_Callback
    {
        get { return on_use_skill_callback; }
        set 
        {
            on_use_skill_callback = value; 
        }
    }
    private bool is_cool_time = false;
    public bool Is_Cool_Time { get { return is_cool_time; } }
    
    private float remaining_cool_time = 0f;
    public float Remaining_Cool_Time { get { return remaining_cool_time; } }
    public void DoCoolTime()
    {
        MonoBehaviour mono = GameManager.instance.BorrowMono();

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
}
public class SkillBookManager : MonoBehaviour
{
    [SerializeField]
    private SkillInfo[] skill_info_list;
    [SerializeField]
    private GameObject skill_prefab;
    [SerializeField]
    private GameObject skill_scroll_view_content;
    [SerializeField]
    private Text remaining_skill_point_text;
    [SerializeField]
    private int remaining_skill_point;
    [SerializeField]
    private UnitStats player_stats;
    [SerializeField]
    private SkillDragAndDropContainer skill_drag_and_drop_container;
    [SerializeField]
    private Transform skill_slots_container;
    [SerializeField]
    private InteractionUIEvent interaction_ui_event;

    private const int NOT_FOUND = -1;
    private void Awake()
    {
        CreateSkillList();
        InitRemainingSkillPoint();
        player_stats.On_Level_Up += CheckSkillUpgradePossible;
        CheckSkillUpgradePossible();
    }
    void CreateSkillList()
    {
        for (int i = 0; i < skill_info_list.Length; i++)
        {
            GameObject skill_obj = Instantiate(skill_prefab, skill_scroll_view_content.transform);

            skill_obj.GetComponent<SkillBookSlot>().Init(skill_info_list[i], skill_drag_and_drop_container, this, interaction_ui_event);
            
            Transform skill_obj_background = skill_obj.transform.GetChild(0);

            skill_obj_background.GetChild(0).GetComponent<Image>().sprite = skill_info_list[i].icon;
            skill_obj_background.GetChild(1).GetComponent<Text>().text = skill_info_list[i].name;
            skill_obj_background.GetChild(2).GetComponent<Text>().text = skill_info_list[i].taken_point.ToString();

            EventTrigger.Entry entry = new EventTrigger.Entry();

            entry.eventID = EventTriggerType.PointerClick;
            int skill_idx = i;
            entry.callback.AddListener((eventData) => { PushSkillUpgradBtn(eventData, skill_idx); });

            skill_obj_background.GetChild(3).GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }
    void InitRemainingSkillPoint()
    {
        remaining_skill_point_text.text = remaining_skill_point.ToString();
    }

    void CheckSkillUpgradePossible()
    {
        int my_level = player_stats.GetLevel;

        for(int i = 0; i < skill_info_list.Length; i++)
        {
            skill_scroll_view_content.transform.GetChild(i).GetChild(0).GetChild(3).GetComponent<Button>().
                interactable = (my_level >= skill_info_list[i].possible_level);

            CheckSkillMaster(i);
        }
    }
    void PushSkillUpgradBtn(BaseEventData event_data, int idx)
    {
        if (skill_scroll_view_content.transform.GetChild(idx).GetChild(0).GetChild(3).GetComponent<Button>().interactable == false) return;
        if (((PointerEventData)event_data).button != PointerEventData.InputButton.Left) return;

        if(remaining_skill_point <= 0)
        {
            Debug.Log("스킬 포인트 부족.");

            return;
        }

        remaining_skill_point_text.text = (--remaining_skill_point).ToString();

        int updated_taken_point = ++skill_info_list[idx].taken_point;

        skill_scroll_view_content.transform.GetChild(idx).GetChild(0).GetChild(2).GetComponent<Text>().text = (updated_taken_point).ToString();
        skill_scroll_view_content.transform.GetChild(idx).GetComponent<SkillBookSlot>().UpdateSkillTakenPoint(updated_taken_point);

        CheckSkillMaster(idx);
    }

    void CheckSkillMaster(int idx)
    {
        if (skill_info_list[idx].limit_point <= skill_info_list[idx].taken_point)
        {
            skill_scroll_view_content.transform.GetChild(idx).GetChild(0).GetChild(3).GetComponent<Button>().interactable = false;
        }
    }
    
    public void AddSkill(SkillInfo skill_info)
    {
        if (GetIdxOfAlreadySkillRegistered(skill_info.name) != NOT_FOUND) return;

        int idx = GetIdxOfEmptySkillSlot();

        if (idx == NOT_FOUND) AddSkillToSkillSlot(0, skill_info); // 빈 공간이 없다면 제일 스킬슬롯들 중 제일 왼쪽 슬록에 등록
        else AddSkillToSkillSlot(idx, skill_info);
    }
    void AddSkillToSkillSlot(int slot_idx, SkillInfo skill_info)
    {
        skill_slots_container.GetChild(slot_idx).GetComponent<SkillSlot>().RemoveSlotUI();
        skill_slots_container.GetChild(slot_idx).GetComponent<SkillSlot>().UpdateSlotUI(skill_info);
    }

    private int GetIdxOfAlreadySkillRegistered(string skill_name)
    {
        int result = NOT_FOUND;

        for (int i = 0; i < skill_slots_container.childCount; i++)
        {
            SkillSlot skill_slot = skill_slots_container.GetChild(i).GetComponent<SkillSlot>();

            if (skill_slot.GetSkillName().Equals(skill_name) == true)
            {
                result = i;

                break;
            }
        }

        return result;
    }

    private int GetIdxOfEmptySkillSlot()
    {
        int result = NOT_FOUND;

        for (int i = 0; i < skill_slots_container.childCount; i++)
        {
            SkillSlot skill_slot = skill_slots_container.GetChild(i).GetComponent<SkillSlot>();

            if (skill_slot.IsEmpty() == true)
            {
                result = i;

                break;
            }
        }

        return result;
    }

    public void RemoveAlreadySkillRegistered(string skill_name)
    {
        for (int i = 0; i < skill_slots_container.childCount; i++)
        {
            SkillSlot skill_slot = skill_slots_container.GetChild(i).GetComponent<SkillSlot>();

            if (skill_slot.GetSkillName().Equals(skill_name) == true)
            {
                skill_slot.RemoveSlotUI();

                return;
            }
        }
    }
}
