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
    private bool is_dragging;
    protected Item item;

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
        StartCoroutine(RemoveSlotUICor());
    }
    IEnumerator RemoveSlotUICor()
    {
        Debug.Log("������ ���Գ�: " + slot_num);

        yield return null;
        item = null;
        item_image.sprite = null;
        item_image.gameObject.SetActive(false);
        item_mount_state_text.gameObject.SetActive(false);
        Is_Mount = false;
    }

    public virtual void UpdateSlotUI(Item item)
    {
        StartCoroutine(UpdateSlotUICor(item));
    }

    IEnumerator UpdateSlotUICor(Item item)
    {
        yield return null;

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
        // �ش� �������� �Һ� �������̸� �������̶�� ������ ���Կ��� ���� ����ȭ ���� ��
        if(item.item_type == ItemType.Potion && Is_Mount == true)
        {
            InventoryManager.instance.ReportChangedItemCntToPotionSlot(item.item_key, cnt);
        }
    }

    public virtual void OnPointerUp(PointerEventData pointer_event_data)
    {
        if (pointer_event_data.button != PointerEventData.InputButton.Right) return;
        if (item == null) return;
        Debug.Log("1 Ÿ��: " + item.item_type);
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

    public virtual void OnPointerEnter(PointerEventData event_data)
    {
        if (item == null) return;

        interaction_ui_event.Show_Item_Info_UI(item, GetComponent<RectTransform>().position);
    }
    public virtual void OnPointerExit(PointerEventData event_data)
    {
        interaction_ui_event.Hide_Item_Info_UI();
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
        Debug.Log("2 ���� ���� ����: " + before_mount_state);

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
    }

    public virtual void OnDrag(PointerEventData event_data)
    {
        if (is_dragging == false) return;

        drag_and_drop_container.transform.position = event_data.position;
    }
    public virtual void OnEndDrag(PointerEventData event_data)
    {
        StartCoroutine(OnEndDragCor());
    }

    IEnumerator OnEndDragCor()
    {
        yield return null;

        if (is_dragging == true)
        {
            Debug.Log("���� �巡�� ���� ��. ���� ��: " + Slot_Num + ", �ٲ� ��: " + drag_and_drop_container.slot_num);
            if (drag_and_drop_container.item != null)
            {
                yield return null;

                UpdateSlotUI(drag_and_drop_container.item);
                Is_Mount = drag_and_drop_container.is_mount;
                Slot_Num = drag_and_drop_container.slot_num;

                //int potion_slot_num = drag_and_drop_container.potion_slot_num;

                yield return null;

                if (Is_Mount == true)
                {
                    if (item.item_type == ItemType.Potion)
                    {
                        InventoryManager.instance.ReportSlotNumToPotionSlotNum(item.item_key, Slot_Num); // ��ȸ�˻�
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
                yield return null;
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
    public virtual void OnDrop(PointerEventData event_data) // ��� ������Ʈ�� �ص� �巡�� �̺�Ʈ ���� ��� ������Ʈ���� ��� �̺�Ʈ�� ���� �߻���.
    {
        if (GetComponent<Button>().interactable == false) return;
        StartCoroutine(OnDropCor());
    }

    IEnumerator OnDropCor()
    {
        yield return null;

        if (drag_and_drop_container.item != null)
        {
            Item temp_item = item;
            Sprite temp_sprite = item_image.sprite;
            bool is_mount = Is_Mount;
            int slot_num = Slot_Num;
            
            yield return null;

            UpdateSlotUI(drag_and_drop_container.item);
            Is_Mount = drag_and_drop_container.is_mount;
            Slot_Num = drag_and_drop_container.slot_num;
            
            yield return null;

            drag_and_drop_container.item = temp_item;
            drag_and_drop_container.item_image.sprite = temp_sprite;
            drag_and_drop_container.slot_num = slot_num;
            Debug.Log("��� ���� ��. ���� ��: " + slot_num + ", �ٲ� ��: " + Slot_Num);
            drag_and_drop_container.is_mount = is_mount;

            yield return null;

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
    }
}
