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
    private GameObject raid_map_ui;

    #region ������ ����Ʈ
    [SerializeField]
    private GameObject item_info_ui_group;
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
    private Text item_mp_recovery_amount_text;
    [SerializeField]
    private Text item_cool_time_text;
    #endregion

    #region ��ų ����Ʈ
    [SerializeField]
    private GameObject skill_info_ui_group;
    [SerializeField]
    private Image skill_image;
    [SerializeField]
    private Text skill_name_text;
    [SerializeField]
    private Text skill_possible_level_text;
    [SerializeField]
    private Text skill_cool_time_text;
    [SerializeField]
    private Text skill_mp_cost_text;
    [SerializeField]
    private Text skill_taken_point_text;
    [SerializeField]
    private Text skill_limit_point_text;
    [SerializeField]
    private Text skill_explain_text;
    [SerializeField]
    private Text skill_damage_text;
    #endregion

    private void Start()
    {
        interaction_event.Get_Field_Item_Text_UI = PopUpFieldItemGetTextUI;
        interaction_event.Show_Acquired_Item_UI = PopUpAcquiredItemUI;
        
        interaction_event.Show_Item_Info_UI = ShowItemInfoUI;
        interaction_event.Hide_Item_Info_UI = HideItemInfoUI;

        interaction_event.Show_Skill_Info_UI = ShowSkillInfoUI;
        interaction_event.Hide_Skill_Info_UI = HideSkillInfoUI;

        interaction_event.On_Change_Visible_Raid_Map_UI = ChangeVisibleRaidMapUI;
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
        SetInfoUIPosition(item_info_ui_group, mouse_position);

        item_image.sprite = item.item_image;
        item_image.SetNativeSize();

        if (item.item_type == ItemType.Sword) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Shield) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Potion) item_type_text.text = "Ÿ��: " + "����";
        else if (item.item_type == ItemType.Helmet) item_type_text.text = "Ÿ��: " + "���";

        item_name_text.text = item.name;
        item_str_text.text = "���ݷ�: " + item.str.ToString();
        item_def_text.text = "����: " + item.def.ToString();
        item_hp_recovery_amount_text.text = "ü�� ȸ����: " + item.hp_recovery_amount.ToString();
        item_mp_recovery_amount_text.text = "���� ȸ����: " + item.mp_recovery_amount.ToString();
        item_cool_time_text.text = "���� ���ð�: " + item.cool_time.ToString() + "��";

        // ��ġ�� 0�� ������ ������ ǥ���� �ʿ� ����.
        item_str_text.gameObject.SetActive(item.str != 0);
        item_def_text.gameObject.SetActive(item.def != 0);
        item_hp_recovery_amount_text.gameObject.SetActive(item.hp_recovery_amount != 0);
        item_mp_recovery_amount_text.gameObject.SetActive(item.mp_recovery_amount != 0);
        item_cool_time_text.gameObject.SetActive(item.cool_time != 0);
    }

    private void SetInfoUIPosition(GameObject info_ui_group, Vector3 mouse_position)
    {
        info_ui_group.SetActive(true);

        RectTransform rect_transform = info_ui_group.transform.GetChild(0).GetComponent<RectTransform>();
        Rect rect = rect_transform.rect;

        float offset_x = (rect.width / 2) * rect_transform.localScale.x;
        float offset_y = (rect.height / 2) * rect_transform.localScale.y;

        info_ui_group.transform.GetChild(0).position = mouse_position - new Vector3(offset_x, offset_y, 0); // ����â ui ���� ���� �ϴ����� ui ��ħ
        info_ui_group.transform.GetChild(0).position = GetFullyVisibleItemInfoUIZone(rect_transform, offset_x, offset_y);
    }
    private void HideItemInfoUI()
    {
        item_info_ui_group.SetActive(false);
    }

    private void ShowSkillInfoUI(SkillInfo skill_info, Vector3 mouse_position)
    {
        SetInfoUIPosition(skill_info_ui_group, mouse_position);

        skill_image.sprite = skill_info.icon;
        //skill_image.SetNativeSize();

        skill_name_text.text = skill_info.name;
        skill_possible_level_text.text = "LV: " + skill_info.possible_level.ToString();
        skill_cool_time_text.text = "���� ���ð�: " + skill_info.cool_time.ToString() + "��";
        skill_mp_cost_text.text = "���� �Ҹ�: " + skill_info.mp_cost.ToString();
        //skill_taken_point_text.text = "���� ����Ʈ: " + skill_info.taken_point.ToString();
        skill_taken_point_text.text = "Point: (" + skill_info.taken_point + "/" + skill_info.limit_point + ")";
        //skill_limit_point_text.text = "������ ����Ʈ: " + skill_info.limit_point.ToString();
        skill_damage_text.text = "���ݷ��� " + ((int)(skill_info.damage_ratio * 100)).ToString() + "%";
        skill_explain_text.text = skill_info.explain.ToString();

        skill_mp_cost_text.gameObject.SetActive(skill_info.mp_cost != 0);
        skill_damage_text.gameObject.SetActive(skill_info.damage_ratio != 0f);
    }

    private void HideSkillInfoUI()
    {
        skill_info_ui_group.SetActive(false);
    }

    void ChangeVisibleRaidMapUI(bool value)
    {
        raid_map_ui.SetActive(value);
    }
}
