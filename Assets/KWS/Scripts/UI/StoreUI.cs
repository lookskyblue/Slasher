using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StoreUI : MonoBehaviour, IDropHandler
{
    private DragAndDropContainer drag_and_drop_container;
    [SerializeField] private InteractionUIEvent interaction_ui_event;
    [SerializeField] private GameObject store_ui;
    [SerializeField] private ItemDB item_db;
    [SerializeField] private GameObject item_ui_prefab;
    [SerializeField] private Transform item_list_content;
    [SerializeField] private string purchase_input_field_amount_alert;
    [SerializeField] private string purchase_not_enough_money_alert;
    [SerializeField] private string already_mounted_item_alert;
    [SerializeField] private string Insufficient_sales_quantity_alert;
    private Item tmp_item;
    private float resale_ratio;
    private int slot_num;

    #region 구매진행 팝업 UI
    [SerializeField] private GameObject purchase_progress_pop_up_ui;
    [SerializeField] private Image purchase_item_icon;
    [SerializeField] private InputField purchase_amount_input_field;
    [SerializeField] private Button purchase_button;
    [SerializeField] private Text button_text;
    #endregion
    private void Start()
    {
        PushCloseStore();

        CreateSaleItemList(ItemType.Potion);
        CreateSaleItemList(ItemType.Sword);
        CreateSaleItemList(ItemType.Shield);

        drag_and_drop_container = GameManager.instance.GetDragAndDropContainer;
        resale_ratio = GameManager.instance.GetResaleRatio;
    }

    void CreateSaleItemList(ItemType item_type)
    {
        List<Item> item_list = item_db.GetItemList(item_type);

        for (int i = 0; i < item_list.Count; i++)
        {
            SaleItem sale_item = Instantiate(item_ui_prefab, item_list_content).GetComponent<SaleItem>();

            sale_item.Init(item_list[i], this);
        }
    }
    public void OpenStore()
    {
        GameManager.instance.Is_Using_Store = true;
        store_ui.SetActive(true);
    }
    public void PushCloseStore()
    {
        GameManager.instance.Is_Using_Store = false;

        interaction_ui_event.Hide_Item_Info_UI.Invoke();
        tmp_item = null;
        store_ui.SetActive(false);

        PushPurchasingCancelButton();
    }
    public void ReportBuyItem(Item tmp_item)
    {
        this.tmp_item = tmp_item;

        PopUpUI("구매하기", PushPurchasingButton, tmp_item);
    }

    void PopUpUI(string button_text, UnityAction func, Item item)
    {
        this.button_text.text = button_text;

        purchase_button.onClick.RemoveAllListeners();
        purchase_button.onClick.AddListener(func);

        purchase_item_icon.sprite = item.item_image;
        purchase_item_icon.SetNativeSize();
        purchase_progress_pop_up_ui.SetActive(true);
        purchase_amount_input_field.gameObject.SetActive(item.is_stackable == true);
    }

    public void PushPurchasingButton()
    {
        if (tmp_item.is_stackable == true)
        {
            if (IsValidInputField() == false)
            {
                interaction_ui_event.On_Change_Alert_Text_UI(purchase_input_field_amount_alert);

                return;
            }
        }

        if (IsEnoughMoney() == false)
        {
            interaction_ui_event.On_Change_Alert_Text_UI(purchase_not_enough_money_alert);

            return;
        }

        int item_cnt = GetValidCntOfInputFieldValue();

        InventoryUI.instance.OnInventoryUI();
        if (InventoryManager.instance.AddItem(tmp_item, item_cnt) == false) return;
        InventoryManager.instance.Gold_On_Hand -= (tmp_item.price * item_cnt);

        //Debug.Log("구매한 아이템명: " + tmp_item.name + ", 수량: " + item_cnt);
        PushPurchasingCancelButton();
    }
    bool IsValidInputField()
    {
        string input_field_value = purchase_amount_input_field.text;
        int value = GetInputFieldValue();

        if (string.IsNullOrEmpty(input_field_value) == true || (value <= 0 || 100 < value)) return false;
        else return true;
    }
    int GetInputFieldValue() { return int.Parse(purchase_amount_input_field.text); }
    int GetValidCntOfInputFieldValue()
    {
        int item_cnt = 1;
        if (tmp_item.is_stackable == true) item_cnt = GetInputFieldValue();

        return item_cnt;
    }
    bool IsEnoughMoney()
    {
        if (InventoryManager.instance.Gold_On_Hand < (tmp_item.price * GetValidCntOfInputFieldValue())) return false;
        else return true;
    }
    public void PushPurchasingCancelButton()
    {
        purchase_progress_pop_up_ui.SetActive(false);
        purchase_item_icon.sprite = null;
        purchase_amount_input_field.text = "1";
    }
    public void OnDrop(PointerEventData event_data)
    {
        if (drag_and_drop_container.item == null) return;
        if (drag_and_drop_container.is_mount == true)
        {
            interaction_ui_event.On_Change_Alert_Text_UI(already_mounted_item_alert);

            return;
        }

        tmp_item = drag_and_drop_container.item;
        slot_num = drag_and_drop_container.slot_num;

        PopUpUI("판매하기", PushSellButton, tmp_item);
    }

    public void PushSellButton()
    {
        if (tmp_item.is_stackable == true)
        {
            if (IsValidInputField() == false)
            {
                interaction_ui_event.On_Change_Alert_Text_UI(purchase_input_field_amount_alert);

                return;
            }
        }

        int item_cnt = GetValidCntOfInputFieldValue();

        // 실수량과 비교
        if(tmp_item.item_cnt < item_cnt)
        {
            interaction_ui_event.On_Change_Alert_Text_UI(Insufficient_sales_quantity_alert);

            return;
        }

        InventoryManager.instance.RemoveItem(tmp_item, slot_num, item_cnt);
        int resale_price = ((int)(tmp_item.price * resale_ratio)) * item_cnt;
        InventoryManager.instance.Gold_On_Hand += resale_price; // 더해야함

        PushPurchasingCancelButton();
    }
}
