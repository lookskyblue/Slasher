using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaidBoardManager : MonoBehaviour
{
    [SerializeField]
    private InteractionUIEvent interaction_ui_event;
    private string selected_raid_name;

    [SerializeField]
    private GameObject raid_map_ui;
    [SerializeField]
    private GameObject popup_ui;

    private void Start()
    {
        TriggerCallback trigger_callback = GetComponent<TriggerCallback>();

        trigger_callback.collision_enter_event = OnOpenRaidMapUI;
        trigger_callback.collision_exit_event = OnCloseRaidMapUI;
    }

    void OnOpenRaidMapUI(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        interaction_ui_event.On_Change_Visible_Raid_Map_UI(true);
    }
    void OnCloseRaidMapUI(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        interaction_ui_event.On_Change_Visible_Raid_Map_UI(false);
    }

    public void PushWantRaidIconButton(string raid_name)
    {
        selected_raid_name = raid_name;
        popup_ui.SetActive(true);
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
        selected_raid_name = string.Empty;
        popup_ui.SetActive(false);
    }
}
