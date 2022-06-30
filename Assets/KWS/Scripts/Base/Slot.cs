using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerUpHandler, IBeginDragHandler,IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected DragAndDropContainer drag_and_drop_container;
    [SerializeField] protected InteractionUIEvent interaction_ui_event;
    [SerializeField] protected Image item_image;
    [SerializeField] protected Text item_cnt_text;
    [SerializeField] protected Text item_mount_state_text;
    [SerializeField] private UnitStats player_stats;
    [SerializeField] private string wearable_level_alert;
    [SerializeField] private AudioClip slot_interaction_sound;
    private AudioSource audio_source;
    private int slot_idx;
    private bool is_dragging;
    protected Item item;

    public int Slot_Idx
    {
        get { return slot_idx; }
        set { slot_idx = value; }
    }

    private int slot_num;
    public int Slot_Num
    {
        get { return slot_num; }
        set { slot_num = value; }
    }

    private bool is_mount = false;
    public bool Is_Mount
    {
        get { return is_mount; }
        set { is_mount = value; }
    }

    void Awake()
    {
        audio_source = GetComponent<AudioSource>();
    }
    public virtual void RemoveSlotUI()
    {
        //StartCoroutine(RemoveSlotUICor());
        item = null;
        item_image.sprite = null;
        item_image.gameObject.SetActive(false);

        if (item_mount_state_text != null) item_mount_state_text.gameObject.SetActive(false);

        Is_Mount = false;

        InventoryUI.instance.SaveInventory();
    }
    IEnumerator RemoveSlotUICor()
    {
        yield return null;

        item = null;
        item_image.sprite = null;
        item_image.gameObject.SetActive(false);

        if (item_mount_state_text != null) item_mount_state_text.gameObject.SetActive(false);

        Is_Mount = false;

        InventoryUI.instance.SaveInventory();
    }

    public virtual void UpdateSlotUI(Item item)
    {
        //StartCoroutine(UpdateSlotUICor(item));

        this.item = item;
        this.item.item_type = item.item_type;

        this.item_image.sprite = item.item_image;
        this.item_image.gameObject.SetActive(true);

        if (item_cnt_text != null)
        {
            this.item_cnt_text.text = item.item_cnt.ToString();
            item_cnt_text.gameObject.SetActive(item.is_stackable == true);
        }

        item_image.SetNativeSize();

        InventoryUI.instance.SaveInventory();
    }

    IEnumerator UpdateSlotUICor(Item item)
    {
        //yield return null;

        this.item = item;
        this.item.item_type = item.item_type;

        yield return null;

        this.item_image.sprite = item.item_image;
        this.item_image.gameObject.SetActive(true);

        yield return null;

        if (item_cnt_text != null)
        {
            this.item_cnt_text.text = item.item_cnt.ToString();
            item_cnt_text.gameObject.SetActive(item.is_stackable == true);
        }

        item_image.SetNativeSize();
    }

    public virtual void UpdateItemCnt(int cnt)
    {
        item_cnt_text.text = cnt.ToString();
        // 해당 아이템이 소비 아이템이며 장착중이라면 장착된 슬롯에도 개수 동기화 해줄 것
        if(item.item_type == ItemType.Potion && Is_Mount == true)
        {
            InventoryManager.instance.ReportChangedItemCntToPotionSlot(item.item_key, cnt);
        }

        InventoryUI.instance.SaveInventory();
    }

    public virtual void OnPointerUp(PointerEventData pointer_event_data)
    {
        if (pointer_event_data.button != PointerEventData.InputButton.Right) return;
        if (item == null) return;
        if(player_stats.GetLevel < item.wearable_level)
        {
            interaction_ui_event.On_Change_Alert_Text_UI(wearable_level_alert);

            return;
        }

        StartCoroutine(CheckItem(item.item_type));
        audio_source.PlayOneShot(slot_interaction_sound);
    }

    IEnumerator CheckItem(ItemType item_type) // 중요
    {
        switch (item_type)
        {
            case ItemType.Sword:
            case ItemType.Shield:
                {
                    bool before_mount_state = Is_Mount;

                    UnmountEquipmentItem(item_type);
                    yield return null;
                    MountEquipmentItem(item_type, before_mount_state);

                    break;
                }

            case ItemType.Potion:
                if (Is_Mount == false) // 마영전도 소비템은 장착중일 때 우클릭 안 먹음.
                {
                    MountPotionItem();
                }

                break;

            default :
                {
                    break;
                }
        }
    }

    public virtual void OnPointerEnter(PointerEventData event_data)
    {
        if (item == null) return;

        interaction_ui_event.Show_Item_Info_UI(item, GetComponent<RectTransform>().position);
    }
    public virtual void OnPointerExit(PointerEventData event_data)
    {
        interaction_ui_event.Hide_Item_Info_UI();
    }

    protected void UnmountEquipmentItem(ItemType item_type) // 중요
    {
        InventoryManager.instance.UnmountItem(item_type);
    }
    private void MountPotionItem() // 포션 착용 함수
    {
        Is_Mount = true;
        item_mount_state_text.gameObject.SetActive(Is_Mount);
        InventoryManager.instance.MountItem(item, Slot_Num); // 꽉찼다면 처음 슬롯부터 넣자.
    }
    void MountEquipmentItem(ItemType item_type, bool before_mount_state) // 장비 착용 함수
    {
        if (before_mount_state == true)
        {
            return;
        }

        Is_Mount = true;
        item_mount_state_text.gameObject.SetActive(Is_Mount);
        InventoryManager.instance.MountItem(item_type, item, Slot_Num);
    }

    public void MountItem(ItemType item_type)
    {
        if(item_type == ItemType.Potion)
        {
            MountPotionItem();
        }

        if(item_type == ItemType.Sword || item_type == ItemType.Shield)
        {
            MountEquipmentItem(item_type, false);
        }
    }

    public virtual void OnBeginDrag(PointerEventData event_data)
    {
        if (item == null) return;
        
        if (is_dragging == false && event_data.button == PointerEventData.InputButton.Right)
        {
            drag_and_drop_container.item = null;
            return;
        }

        drag_and_drop_container.item = item;
        drag_and_drop_container.item_image.sprite = item_image.sprite;
        drag_and_drop_container.item_image.SetNativeSize();
        drag_and_drop_container.item_image.gameObject.SetActive(true);
        drag_and_drop_container.slot_num = Slot_Num; // 1
        drag_and_drop_container.is_mount = Is_Mount;

        is_dragging = true;

        audio_source.PlayOneShot(slot_interaction_sound);
    }

    public virtual void OnDrag(PointerEventData event_data)
    {
        if (is_dragging == false) return;

        drag_and_drop_container.transform.position = event_data.position;
    }
    //private void Update()
    //{
    //    if (item == null) return;

    //    Debug.Log("아이템명: " + item.name + ", 슬롯넘: " + Slot_Num);
    //}
    public virtual void OnEndDrag(PointerEventData event_data)
    {
        if (is_dragging == true)
        {
            if (drag_and_drop_container.item != null)
            {
                UpdateSlotUI(drag_and_drop_container.item);
                Is_Mount = drag_and_drop_container.is_mount;
                Slot_Num = drag_and_drop_container.slot_num;

                //int potion_slot_num = drag_and_drop_container.potion_slot_num;

                if (Is_Mount == true)
                {
                    if (item.item_type == ItemType.Potion)
                    {
                        InventoryManager.instance.ReportSlotNumToPotionSlotNum(item.item_key, Slot_Num); // 순회검사
                    }

                    else
                        InventoryManager.instance.ReportSlotNumToEquipmentSlotNum(item.item_type, Slot_Num);

                    if (item_mount_state_text != null)
                        item_mount_state_text.gameObject.SetActive(true);
                }

                else
                {
                    if (item_mount_state_text != null)
                        item_mount_state_text.gameObject.SetActive(false);
                }
            }

            else
            {
                RemoveSlotUI();
                Is_Mount = false;
                Slot_Num = drag_and_drop_container.slot_num;
            }
        }

        is_dragging = false;
        drag_and_drop_container.item = null;
        drag_and_drop_container.item_image.sprite = null;
        drag_and_drop_container.item_image.gameObject.SetActive(false);
        drag_and_drop_container.slot_num = -1;
        drag_and_drop_container.is_mount = false;
    }
    public virtual void OnDrop(PointerEventData event_data) // 드랍 오브젝트의 앤드 드래그 이벤트 보다 드롭 오브젝트에서 드롭 이벤트가 먼저 발생함.
    {
        if (GetComponent<Button>().interactable == false) return;

        if (drag_and_drop_container.item != null)
        {
            Item temp_item = item;
            
            Sprite temp_sprite = item_image.sprite;
            bool is_mount = Is_Mount;
            int slot_num = Slot_Num;
            UpdateSlotUI(drag_and_drop_container.item);
            Is_Mount = drag_and_drop_container.is_mount;
            Slot_Num = drag_and_drop_container.slot_num;

            drag_and_drop_container.item = temp_item;
            drag_and_drop_container.item_image.sprite = temp_sprite;
            drag_and_drop_container.slot_num = slot_num;
            drag_and_drop_container.is_mount = is_mount;

            if (Is_Mount == true)
            {
                if (item != null && item.item_type != ItemType.Potion)
                    InventoryManager.instance.ReportSlotNumToEquipmentSlotNum(item.item_type, Slot_Num);

                if (item_mount_state_text != null)
                    item_mount_state_text.gameObject.SetActive(true);
            }

            else
            {
                if (item_mount_state_text != null)
                    item_mount_state_text.gameObject.SetActive(false);
            }
        }

        else
        {
            drag_and_drop_container.item = null;
            drag_and_drop_container.item_image.sprite = null;
            drag_and_drop_container.is_mount = false;
            drag_and_drop_container.slot_num = -1;
        }

        audio_source.PlayOneShot(slot_interaction_sound);
    }
}
