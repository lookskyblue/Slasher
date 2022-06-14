using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SkillSlot : SkillBookSlot
{
    [SerializeField]
    private Sprite default_image;
    [SerializeField]
    private Image image;
    [SerializeField]
    private KeyCode key_code;
    [SerializeField]
    private Text key_code_text;
    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;
    [SerializeField]
    private Image cool_time_ui;
    private void Start()
    {
        InitKeyCodeText();
    }
    void InitKeyCodeText()
    {
        string key_code_text_str = key_code.ToString();
        key_code_text.text = key_code_text_str.Substring(key_code_text_str.Length - 1);
    }

    public void UpdateSlotUI(SkillInfo skill_info) 
    {
        this.skill_info = skill_info;
        this.skill_info.On_Use_Skill_Callback = OnSkillInputCallback;

        image.sprite = skill_info.icon;
        interaction_slot_event.MountSkillSlot.Invoke(key_code, this.skill_info);
    }
    public void RemoveSlotUI() 
    {
        if (skill_info == null) return;

        skill_info.On_Use_Skill_Callback = null;
        skill_info = null;
        image.sprite = default_image;

        interaction_slot_event.UnmountSkillSlot.Invoke(key_code);
    }
    public void OnSkillInputCallback()
    {
        StartCoroutine(UpdateCoolTimeUI());
    }
    IEnumerator UpdateCoolTimeUI()
    {
        if(skill_info.Is_Cool_Time == true)
        {
            cool_time_ui.gameObject.SetActive(true);

            float remaining_cool_time = skill_info.Remaining_Cool_Time;
            float max_cool_time = skill_info.cool_time;

            while(remaining_cool_time < max_cool_time)
            {
                remaining_cool_time += Time.deltaTime;
                cool_time_ui.fillAmount = 1 - remaining_cool_time / max_cool_time;

                yield return null;
            }

            cool_time_ui.gameObject.SetActive(false);
        }
    }
    public override void OnPointerUp(PointerEventData event_data)
    {
        interaction_slot_event.UsingSkillSlot.Invoke(false);

        if (event_data.button != PointerEventData.InputButton.Right) return;
        if (skill_info == null) return;

        RemoveSlotUI();
    }
    public override void OnDrop(PointerEventData event_data)
    {
        interaction_slot_event.UsingSkillSlot.Invoke(false);

        if (skill_drag_and_drop_container.Skill_Info == null) return;
        if (IsValidSkillToRegister(skill_drag_and_drop_container.Skill_Info) == false) return;

        SkillInfo temp_skill_info = skill_info;

        if (skill_info != null)
            RemoveSlotUI();

        skill_book_manager. RemoveAlreadySkillRegistered(skill_drag_and_drop_container.Skill_Info.name);
        UpdateSlotUI(skill_drag_and_drop_container.Skill_Info);

        skill_drag_and_drop_container.Skill_Info = temp_skill_info;
    }

    private bool IsValidSkillToRegister(SkillInfo skill_info)
    {
        if(skill_info.taken_point <= 0)
        {
            Debug.Log("아직 배우지 않은 스킬입니다.");

            return false;
        }

        return true;
    }
    public override void OnBeginDrag(PointerEventData event_data)
    {
        base.OnBeginDrag(event_data);
        interaction_slot_event.UsingSkillSlot.Invoke(true);
    }

    public override void OnDrag(PointerEventData event_data)
    {
        base.OnDrag(event_data);
    }

    public override void OnEndDrag(PointerEventData event_data)
    {
        interaction_slot_event.UsingSkillSlot.Invoke(false);

        if (skill_drag_and_drop_container.Skill_Info == null)
        {
            base.OnEndDrag(event_data);
            return;
        }

        RemoveSlotUI();
        UpdateSlotUI(skill_drag_and_drop_container.Skill_Info);

        base.OnEndDrag(event_data);
    }
    public string GetSkillName()
    {
        if (skill_info == null) return string.Empty;

        return skill_info.name;
    }

    public bool IsEmpty() { return skill_info == null; }
}
