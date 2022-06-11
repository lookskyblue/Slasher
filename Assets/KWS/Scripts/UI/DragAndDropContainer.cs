using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropContainer : MonoBehaviour
{
    public Image item_image;
    [HideInInspector]
    public Item item = null;
    public bool is_mount = false;
    public int slot_num = -1;
    //public int potion_slot_num = -1;

    private void Start()
    {
        item = null;
        item_image.gameObject.SetActive(false);
    }
}
