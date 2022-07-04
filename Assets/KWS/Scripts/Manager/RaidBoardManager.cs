using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaidBoardManager : MonoBehaviour
{
    [SerializeField] private InteractionUIEvent interaction_ui_event;
    [SerializeField] private UnitStats player_stats;
    [SerializeField] private string alert_text_of_not_enough_level;
    [SerializeField] private GameObject raid_map_ui;
    [SerializeField] private GameObject popup_ui;
    [SerializeField] private Text entry_level;
    [SerializeField] private Text compensation_gold;
    [SerializeField] private Text compensation_exp;
    private RaidInfo raid_info = null;

    public void GetTriggerCallback(ref Action<Collider> collider_enter, ref Action<Collider> collider_exit)
    {
        collider_enter = OnOpenRaidMapUI;
        collider_exit = OnCloseRaidMapUI;
    }
    void OnOpenRaidMapUI(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        raid_map_ui.SetActive(true);
        popup_ui.SetActive(false);

        ClearRaidInfo();

        GameManager.instance.ChangeMouseState(CursorLockMode.Confined);
    }
    void OnCloseRaidMapUI(Collider collider)
    {
        if (collider.CompareTag("Player") == false) return;

        raid_map_ui.SetActive(false);
        popup_ui.SetActive(false);

        ClearRaidInfo();
    }

    public void PushWantRaidIconButton(RaidInfo raid_info)
    {
        ClearRaidInfo();
        this.raid_info = raid_info;
        UpdateCompensationUI();
        popup_ui.SetActive(true);
    }

    void UpdateCompensationUI()
    {
        entry_level.text = this.raid_info.Entry_Level == 0 ? "0" : GetThousandCommaText(this.raid_info.Entry_Level);
        compensation_gold.text = this.raid_info.Compensation_Gold == 0 ? "0" : GetThousandCommaText(this.raid_info.Compensation_Gold);
        compensation_exp.text = this.raid_info.Compensation_Exp == 0 ? "0" : GetThousandCommaText(this.raid_info.Compensation_Exp);
    }

    string GetThousandCommaText(int value) { return string.Format("{0:#,###}", value); }

    public void PushEntrySelectedRaidButton()
    {
        if(raid_info == null)
        {
            Debug.LogError("Selected raid is null");

            return;
        }

        if(player_stats.GetLevel < raid_info.Entry_Level)
        {
            PopupAlert();
            
            return;
        }

        raid_map_ui.SetActive(false);
        //SceneManager.LoadScene(raid_info.Raid_Name);
        GameManager.instance.LoadScene(raid_info.Raid_Name);
    }

    public void PushCancelSelectedRaidButton()
    {
        popup_ui.SetActive(false);
        ClearRaidInfo();
    }

    void ClearRaidInfo() { raid_info = null; }

    void PopupAlert()
    {
        interaction_ui_event.On_Change_Alert_Text_UI(alert_text_of_not_enough_level);
    }
}
