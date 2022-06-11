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

    void ActiveOffAllToggleObj()
    {
        for (int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            ui_toggle_key_with_obj[i].ui_toggle_obj.SetActive(false);
            CheckHaveInfoUI(i);
        }
    }
    public void CheckClickOutOfUIs(Vector3 mouse_pos)
    {
        for(int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            RectTransform rect_transform = ui_toggle_key_with_obj[i].ui_toggle_obj.transform.GetChild(0).GetComponent<RectTransform>();

            if (rect_transform.rect.Contains(rect_transform.InverseTransformPoint(mouse_pos)) == true)
                return;
        }

        ActiveOffAllToggleObj();
    }
    private void Update()
    {
        for(int i = 0; i < ui_toggle_key_with_obj.Length; i++)
        {
            if(Input.GetKeyDown(ui_toggle_key_with_obj[i].ui_toggle_key_code) == true)
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
}

