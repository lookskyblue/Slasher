using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaleItem : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private InteractionUIEvent interaction_ui_event;
    [SerializeField] private Image icon;
    [SerializeField] private Text name;
    [SerializeField] private Text price;
    private StoreUI store_ui;
    private Item item_info = null;

    public void Init(Item item, StoreUI store_ui)
    {
        this.store_ui = store_ui;

        item_info = item;

        icon.sprite = item.item_image;
        icon.SetNativeSize();
        name.text = item.name;
        price.text = GetThousandCommaText(item.price);
    }

    public void OnPointerEnter(PointerEventData event_data)
    {
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.x -= (GetComponent<RectTransform>().rect.width * 0.5f);
        //pos.x += 30f;

        interaction_ui_event.Show_Item_Info_UI(item_info, pos);
    }
    public void OnPointerExit(PointerEventData event_data)
    {
        interaction_ui_event.Hide_Item_Info_UI.Invoke();
    }

    public void OnPointerUp(PointerEventData event_data)
    {
        if (event_data.button != PointerEventData.InputButton.Right) return;

        store_ui.ReportBuyItem(item_info.DeepCopy());
    }

    string GetThousandCommaText(int value) { return string.Format("{0:#,###}", value); }
}
