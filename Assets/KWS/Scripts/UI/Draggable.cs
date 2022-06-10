using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 last_mouse_pos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        last_mouse_pos = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 now_mouse_pos = eventData.position;
        Vector2 diff = now_mouse_pos - last_mouse_pos;

        RectTransform rect = GetComponent<RectTransform>();

        Vector3 old_pos = rect.position;
        Vector3 new_pos = rect.position + new Vector3(diff.x, diff.y, transform.position.z);

        rect.position = new_pos;
        
        if (IsRectTransformInsideScreen(rect) == false)
        {
            rect.position = old_pos;
        }

        last_mouse_pos = now_mouse_pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Implement your funtionlity here
    }

    private bool IsRectTransformInsideScreen(RectTransform rect_transform)
    {
        bool is_inside = false;

        Vector3[] corners = new Vector3[4];
        rect_transform.GetWorldCorners(corners);
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        int visible_corners_cnt = 0;

        foreach (Vector3 corner in corners)
        {
            if (rect.Contains(corner))
            {
                visible_corners_cnt++;
            }
        }

        if (visible_corners_cnt == 4)
        {
            is_inside = true;
        }

        return is_inside;
    }
}
