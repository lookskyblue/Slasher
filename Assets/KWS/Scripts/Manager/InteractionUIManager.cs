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

    private void Start()
    {
        interaction_event.Get_Field_Item_Text_UI = PopUpFieldItemGetTextUI;
        interaction_event.Show_Acquired_Item_UI = PopUpAcquiredItemUI;
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
}
