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
    private void Start()
    {
        InitKeyCodeText();
    }
    void InitKeyCodeText()
    {
        string key_code_text_str = key_code.ToString();
        key_code_text.text = key_code_text_str.Substring(key_code_text_str.Length - 1);
    }

    public void UpdateSlotUI(SkillInfo? skill_info) 
    {
        this.skill_info = skill_info;
        image.sprite = skill_info.Value.icon;
        
        // 키 등록
    }

    public void RemoveSlotUI() 
    {
        skill_info = null;
        image.sprite = default_image;

        // 키 해제
    }

    public override void OnPointerUp(PointerEventData event_data)
    {
        if (event_data.button != PointerEventData.InputButton.Right) return;
        if (skill_info == null) return;

        RemoveSlotUI();
    }
    public override void OnDrop(PointerEventData event_data)
    {
        if (skill_drag_and_drop_container.Skill_Info == null) return;

        SkillInfo ?temp_skill_info = skill_info;

        if (skill_info != null)
            RemoveSlotUI();

        RemoveAlreadySkillRegistered(skill_drag_and_drop_container.Skill_Info.Value.name);
        UpdateSlotUI(skill_drag_and_drop_container.Skill_Info);

        skill_drag_and_drop_container.Skill_Info = temp_skill_info;
    }
    void RemoveAlreadySkillRegistered(string skill_name)
    {
        Transform parent = this.transform.parent;

        for(int i = 0; i < parent.childCount; i++)
        {
            SkillSlot skill_slot = parent.GetChild(i).GetComponent<SkillSlot>();

            if (skill_slot.GetSkillName().Equals(skill_name) == true)
            {
                skill_slot.RemoveSlotUI();

                return;
            }
        }
    }
    public override void OnBeginDrag(PointerEventData event_data)
    {
        //skill_drag_and_drop_container.Skill_Info = null;
        base.OnBeginDrag(event_data);
    }

    public override void OnDrag(PointerEventData event_data)
    {
        base.OnDrag(event_data);
    }

    public override void OnEndDrag(PointerEventData event_data)
    {
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

        return skill_info.Value.name;
    }
}
