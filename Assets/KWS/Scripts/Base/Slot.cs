using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerUpHandler, IBeginDragHandler,IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    protected DragAndDropContainer drag_and_drop_container;
    private bool is_dragging;

    //[SerializeField]
    protected Item item;
    [SerializeField]
    protected Image item_image;
    [SerializeField]
    protected Text item_cnt_text;
    [SerializeField]
    protected Text item_mount_state_text;

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
    public virtual void RemoveSlotUI()
    {
        item = null;
        item_image.sprite = null;
        item_image.gameObject.SetActive(false);

        Is_Mount = false;
    }

    public virtual void UpdateSlotUI(Item item)
    {
        this.item = item;
        this.item.item_type = item.item_type;
        this.item_image.sprite = item.item_image;
        this.item_image.gameObject.SetActive(true);

        if (item.is_stackable == true)
        {
            this.item_cnt_text.text = item.item_cnt.ToString();
            item_cnt_text.gameObject.SetActive(true);
        }

        item_image.SetNativeSize();
    }

    public virtual void UpdateItemCnt(int cnt)
    {
        item_cnt_text.text = cnt.ToString();
        // �ش� �������� �Һ� �������̸� �������̶�� ������ ���Կ��� ���� ����ȭ ���� ��
        if(item.item_type == ItemType.Potion && Is_Mount == true)
        {
            InventoryManager.instance.ReportChangedItemCntToPotionSlot(item.item_name, cnt);
        }
    }

    public virtual void OnPointerUp(PointerEventData pointer_event_data)
    {
        if (pointer_event_data.button != PointerEventData.InputButton.Right) return;
        if (item == null) return;

        CheckItem(item.item_type);
    }

    private void CheckItem(ItemType item_type) // �߿�
    {
        switch (item_type)
        {
            case ItemType.Sword:
            case ItemType.Shield:
                {
                    bool before_mount_state = Is_Mount;
                    UnmountItem(item_type);
                    MountItem(item_type, before_mount_state);
                    
                    break;
                }

            case ItemType.Potion:
                if (Is_Mount == false) // �������� �Һ����� �������� �� ��Ŭ�� �� ����.
                {
                    MountItem();
                }

                break;

            default :
                {
                    break;
                }
        }
    }

    protected void UnmountItem(ItemType item_type) // �߿�
    {
        InventoryManager.instance.UnmountItem(item_type);
    }
    private void MountItem()
    {
        Is_Mount = true;
        item_mount_state_text.gameObject.SetActive(Is_Mount);
        InventoryManager.instance.MountItem(item, Slot_Num); // ��á�ٸ� ó�� ���Ժ��� ����.
    }
    private void MountItem(ItemType item_type, bool before_mount_state) // �߿�
    {
        if (before_mount_state == true)
        {
            return;
        }

        Is_Mount = true;
        item_mount_state_text.gameObject.SetActive(Is_Mount);
        InventoryManager.instance.MountItem(item_type, item, Slot_Num);
    }

    public virtual void OnBeginDrag(PointerEventData event_data)
    {
        if (item == null) return;

        drag_and_drop_container.item = item;
        drag_and_drop_container.item_image.sprite = item_image.sprite;
        drag_and_drop_container.item_image.SetNativeSize();
        drag_and_drop_container.item_image.gameObject.SetActive(true);
        drag_and_drop_container.slot_num = Slot_Num; // 1
        drag_and_drop_container.is_mount = Is_Mount;

        is_dragging = true;
    }

    public virtual void OnDrag(PointerEventData event_data)
    {
        if (is_dragging == false) return;

        drag_and_drop_container.transform.position = event_data.position;
    }
    public virtual void OnEndDrag(PointerEventData event_data)
    {
        if(is_dragging == true)
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
                        InventoryManager.instance.ReportSlotNumToPotionSlotNum(item.item_name, Slot_Num); // ��ȸ�˻�
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
        //drag_and_drop_container.potion_slot_num = -1;
        drag_and_drop_container.is_mount = false;
    }
    public virtual void OnDrop(PointerEventData event_data) // ��� ������Ʈ�� �ص� �巡�� �̺�Ʈ ���� ��� ������Ʈ���� ��� �̺�Ʈ�� ���� �߻���.
    {
        if (GetComponent<Button>().interactable == false) return;
        if(drag_and_drop_container.item != null)
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
                if (item.item_type != ItemType.Potion)
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
    }
}
