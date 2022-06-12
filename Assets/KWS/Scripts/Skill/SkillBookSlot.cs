using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBookSlot : MonoBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    protected SkillDragAndDropContainer skill_drag_and_drop_container;
    protected SkillInfo? skill_info = null;
    protected bool is_dragging = false;

    public virtual void OnBeginDrag(PointerEventData event_data)
    {
        Debug.Log("비긴 드래그1");
        if (skill_info == null) return;
        Debug.Log("비긴 드래그2");
        if (is_dragging == false && event_data.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("비긴 드래그3");

            skill_drag_and_drop_container.Skill_Info = null;

            return;
        }
        Debug.Log("비긴 드래그4");

        skill_drag_and_drop_container.Skill_Info = skill_info;
        skill_drag_and_drop_container.Image.sprite = skill_info.Value.icon;
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
        Debug.Log("앤드 드래그");

        skill_drag_and_drop_container.Skill_Info = null;
        skill_drag_and_drop_container.Image.sprite = null;
        skill_drag_and_drop_container.Image.gameObject.SetActive(false);
    }

    public virtual void OnDrop(PointerEventData event_data) { }
    public virtual void OnPointerUp(PointerEventData event_data) // 이벤트 콜백 담당할 녀셕이 필요.
    {

    }
    public void Init(SkillInfo skill_info, SkillDragAndDropContainer container)
    {
        this.skill_info = skill_info;
        this.skill_drag_and_drop_container = container;
    }
}
