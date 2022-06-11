using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUIManager : MonoBehaviour
{
    [SerializeField]
    private InteractionUIEvent interaction_event;
    [SerializeField]
    private GameObject get_field_item_text_ui;
    [SerializeField]
    private GameObject acquired_item_ui_group;
    [SerializeField]
    private Image acquired_item_image_ui;
    [SerializeField]
    private GameObject item_info_ui_group;

    #region ������ ����Ʈ
    [SerializeField]
    private Image item_image;
    [SerializeField]
    private Text item_type_text;
    [SerializeField]
    private Text item_name_text;
    [SerializeField]
    private Text item_str_text;
    [SerializeField]
    private Text item_def_text;
    [SerializeField]
    private Text item_hp_recovery_amount_text;
    [SerializeField]
    private Text item_cool_time_text;
    #endregion
    
    private void Start()
    {
        interaction_event.Get_Field_Item_Text_UI = PopUpFieldItemGetTextUI;
        interaction_event.Show_Acquired_Item_UI = PopUpAcquiredItemUI;
        interaction_event.Show_Item_Info_UI = ShowItemInfoUI;
        interaction_event.Hide_Item_Info_UI = HideItemInfoUI;
    }
    private void PopUpFieldItemGetTextUI(bool value)
    {
        get_field_item_text_ui.SetActive(value);
    }

    private void PopUpAcquiredItemUI(Item acquired_item)
    {
        acquired_item_image_ui.sprite = acquired_item.item_image;
        acquired_item_image_ui.SetNativeSize();
        StartCoroutine(PopUpAcquiredItemUI());
    }

    IEnumerator PopUpAcquiredItemUI()
    {
        acquired_item_ui_group.SetActive(true);
        yield return new WaitForSeconds(2f);
        acquired_item_ui_group.SetActive(false);
    }

    Vector3 GetFullyVisibleItemInfoUIZone(RectTransform rect_transform, float offset_x, float offset_y)
    {
        offset_x *= 2;
        offset_y *= 2;

        Vector3 safe_ui_zone_position = rect_transform.transform.position;
        
        Vector3[] corners = new Vector3[4];
        rect_transform.GetWorldCorners(corners);

        for(int i = 0; i < corners.Length; i++)
        {
            if (i == 1 || i == 3) continue;

            if (corners[i].x < 0) safe_ui_zone_position.x += offset_x;
            else if (Screen.width < corners[i].x) safe_ui_zone_position.x -= offset_x;

            if (corners[i].y < 0) safe_ui_zone_position.y += offset_y;
            else if (Screen.height < corners[i].y) safe_ui_zone_position.y -= offset_y;
        }
        
        return safe_ui_zone_position;
    }

    private void ShowItemInfoUI(Item item, Vector3 mouse_position)
    {
        item_info_ui_group.SetActive(true);
        
        RectTransform rect_transform = item_info_ui_group.transform.GetChild(0).GetComponent<RectTransform>();
        Rect rect = rect_transform.rect;

        float offset_x = (rect.width / 2) * rect_transform.localScale.x;
        float offset_y = (rect.height / 2) * rect_transform.localScale.y;

        item_info_ui_group.transform.GetChild(0).position = mouse_position - new Vector3(offset_x, offset_y, 0); // ����â ui ���� ���� �ϴ����� ui ��ħ
        item_info_ui_group.transform.GetChild(0).position = GetFullyVisibleItemInfoUIZone(rect_transform, offset_x, offset_y);

        item_image.sprite = item.item_image;
        item_image.SetNativeSize();

        if (item.item_type == ItemType.Sword) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Shield) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Potion) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Helmet) item_type_text.text = "Ÿ��: " + "���";

        item_name_text.text = "�����۸�" + "\n" + item.item_name;
        item_str_text.text = "���ݷ�: " + item.str.ToString();
        item_def_text.text = "����: " + item.def.ToString();
        item_hp_recovery_amount_text.text = "ȸ����: " + item.hp_recovery_amount.ToString();
        item_cool_time_text.text = "���� ���ð�: " + item.cool_time.ToString() + "��";

        // ��ġ�� 0�� ������ ������ ǥ���� �ʿ� ����.
        item_str_text.gameObject.SetActive(item.str != 0);
        item_def_text.gameObject.SetActive(item.def != 0);
        item_hp_recovery_amount_text.gameObject.SetActive(item.hp_recovery_amount != 0);
        item_cool_time_text.gameObject.SetActive(item.cool_time != 0);
    }
    private void HideItemInfoUI()
    {
        item_info_ui_group.SetActive(false);
    }
}
