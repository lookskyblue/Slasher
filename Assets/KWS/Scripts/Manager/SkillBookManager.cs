using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public struct SkillInfo
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
    public GameObject hit_area;
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

            skill_obj.GetComponent<SkillBookSlot>().Init(skill_info_list[i], skill_drag_and_drop_container);
            
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
        skill_scroll_view_content.transform.GetChild(idx).GetChild(0).GetChild(2).GetComponent<Text>().text = (++skill_info_list[idx].taken_point).ToString();

        CheckSkillMaster(idx);
    }

    void CheckSkillMaster(int idx)
    {
        if (skill_info_list[idx].limit_point <= skill_info_list[idx].taken_point)
        {
            skill_scroll_view_content.transform.GetChild(idx).GetChild(0).GetChild(3).GetComponent<Button>().interactable = false;
        }
    }
}
