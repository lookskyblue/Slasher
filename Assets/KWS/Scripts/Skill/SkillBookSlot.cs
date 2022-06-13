using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBookSlot : MonoBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    protected SkillDragAndDropContainer skill_drag_and_drop_container;
    protected SkillInfo skill_info = null;
    protected bool is_dragging = false;
    [SerializeField]
    protected SkillBookManager skill_book_manager;

    public virtual void OnBeginDrag(PointerEventData event_data)
    {
        if (skill_info == null) return;
        if (is_dragging == false && event_data.button == PointerEventData.InputButton.Right)
        {
            skill_drag_and_drop_container.Skill_Info = null;

            return;
        }

        skill_drag_and_drop_container.Skill_Info = skill_info;
        skill_drag_and_drop_container.Image.sprite = skill_info.icon;
        skill_drag_and_drop_container.Image.gameObject.SetActive(true);

        is_dragging = true;
    }

    public virtual void OnDrag(PointerEventData event_data)
    {
        if (is_dragging == false) return;

        skill_drag_and_drop_container.transform.position = event_data.position;
    }

    public virtual void OnEndDrag(PointerEventData event_data)
    {
        skill_drag_and_drop_container.Skill_Info = null;
        skill_drag_and_drop_container.Image.sprite = null;
        skill_drag_and_drop_container.Image.gameObject.SetActive(false);
    }

    public virtual void OnDrop(PointerEventData event_data) { }
    public virtual void OnPointerUp(PointerEventData event_data)
    {
        if (event_data.button != PointerEventData.InputButton.Right) return;
        if (IsValidSkillToRegister() == false) return;

        skill_book_manager.AddSkill(skill_info);
    }
    public void Init(SkillInfo skill_info, SkillDragAndDropContainer container, SkillBookManager skill_book_manager)
    {
        this.skill_info = skill_info;
        this.skill_drag_and_drop_container = container;
        this.skill_book_manager = skill_book_manager;
    }
    private bool IsValidSkillToRegister()
    {
        if (skill_info.taken_point <= 0)
        {
            Debug.Log("���� ����� ���� ��ų�Դϴ�.");

            return false;
        }

        return true;
    }
    public void UpdateSkillTakenPoint(int updated_taken_point)
    {
        skill_info.taken_point = updated_taken_point;
    }
}
