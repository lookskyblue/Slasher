using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropContainer : MonoBehaviour
{
    [HideInInspector] public Item item = null;
    public Image item_image;
    public int slot_num = -1;
    public bool is_mount = false;

    private void Start()
    {
        item = null;
        item_image.gameObject.SetActive(false);
    }
}
