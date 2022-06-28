using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIToggleKeyWithObj
{
    public KeyCode ui_toggle_key_code;
    public GameObject ui_toggle_obj;
    public GameObject ui_info_obj;
}
public class UIActiveGetKeyInput : MonoBehaviour
{
    [SerializeField]
    private UIToggleKeyWithObj[] ui_toggle_key_with_obj;
    private void Start()
    {
        ActiveOffAllToggleObj();
    }

    public bool ActiveOffAllToggleObj()
    {
        int active_off_cnt = 0;

        for (int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            if (ui_toggle_key_with_obj[i].ui_toggle_obj == null) continue;
            if (ui_toggle_key_with_obj[i].ui_toggle_obj.activeSelf == true) active_off_cnt++;
            ui_toggle_key_with_obj[i].ui_toggle_obj.SetActive(false);
            CheckHaveInfoUI(i);
        }

        return 0 < active_off_cnt;
    }
    public bool CheckClickOutOfUIs(Vector3 mouse_pos)
    {
        for(int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            if (ui_toggle_key_with_obj[i].ui_toggle_obj == null || ui_toggle_key_with_obj[i].ui_toggle_obj.activeSelf == false) continue;

            RectTransform rect_transform = ui_toggle_key_with_obj[i].ui_toggle_obj.transform.GetChild(0).GetComponent<RectTransform>();

            if (rect_transform.rect.Contains(rect_transform.InverseTransformPoint(mouse_pos)) == true)
                return false;
        }

        return true;
    }
    private void Update()
    {
        for(int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            KeyCode key_code = ui_toggle_key_with_obj[i].ui_toggle_key_code;

            if (key_code != KeyCode.None && Input.GetKeyDown(key_code) == true)
            {
                GameObject obj = ui_toggle_key_with_obj[i].ui_toggle_obj;

                obj.SetActive(!obj.activeSelf);

                if (obj.activeSelf == false)
                {
                    CheckHaveInfoUI(i);
                }
            }
        }
    }

    void CheckHaveInfoUI(int idx)
    {
        if (ui_toggle_key_with_obj[idx].ui_info_obj != null)
        {
            ui_toggle_key_with_obj[idx].ui_info_obj.SetActive(false);
        }
    }

    public bool IsActivedUI()
    {
        for(int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            if (ui_toggle_key_with_obj[i].ui_toggle_obj != null && ui_toggle_key_with_obj[i].ui_toggle_obj.activeSelf == true) return true;
        }

        return false;
    }
}