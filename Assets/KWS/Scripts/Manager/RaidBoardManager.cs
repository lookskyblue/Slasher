using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaidBoardManager : MonoBehaviour
{
    private string selected_raid_name;

    [SerializeField]
    private GameObject raid_map_ui;
    [SerializeField]
    private GameObject popup_ui;

    public void GetTriggerCallback(ref Action<Collider> collider_enter, ref Action<Collider> collider_exit)
    {
        collider_enter = OnOpenRaidMapUI;
        collider_exit = OnCloseRaidMapUI;
    }
    void OnOpenRaidMapUI(Collider collider)
    {
        Debug.Log("Open");

        if (collider.CompareTag("Player") == false) return;

        raid_map_ui.SetActive(true);
        popup_ui.SetActive(false);

        ClearRaidNameCache();
    }
    void OnCloseRaidMapUI(Collider collider)
    {
        Debug.Log("Close");

        if (collider.CompareTag("Player") == false) return;

        raid_map_ui.SetActive(false);
        popup_ui.SetActive(false);

        ClearRaidNameCache();
    }

    public void PushWantRaidIconButton(string raid_name)
    {
        popup_ui.SetActive(true);
        ClearRaidNameCache();

        this.selected_raid_name = raid_name;
    }

    public void PushEntrySelectedRaidButton()
    {
        if(string.IsNullOrEmpty(selected_raid_name) == true)
        {
            Debug.LogError("Selected raid name is null");

            return;
        }

        raid_map_ui.SetActive(false);
        SceneManager.LoadScene(selected_raid_name);
    }

    public void PushCancelSelectedRaidButton()
    {
        popup_ui.SetActive(false);
        ClearRaidNameCache();
    }

    void ClearRaidNameCache() { selected_raid_name = string.Empty; }
}
