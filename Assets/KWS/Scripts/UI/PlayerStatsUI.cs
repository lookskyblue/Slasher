using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    private UnitStats player_stats;

    #region 스탯 정보
    [SerializeField]
    private Text level_text;
    [SerializeField]
    private Text hp_text;
    [SerializeField]
    private Text mp_text;
    [SerializeField]
    private Text str_text;
    [SerializeField]
    private Text def_text;
    #endregion

    private void Start()
    {
        AddCallback();
        RefreshAllStats();
    }

    void AddCallback()
    {
        player_stats.On_Level_Up += RefreshLevelStat;
        player_stats.On_Str_Change += RefrestStrStat;
        player_stats.On_Def_Change += RefrestDefStat;
    }

    void RefreshAllStats()
    {
        int now_hp = player_stats.Total_Hp;
        int now_mp = player_stats.Total_Mp;

        RefreshLevelStat();
        RefrestStrStat();
        RefrestDefStat();
        RefreshLevelStat();

        hp_text.text = "체력:  " + now_hp.ToString();
        mp_text.text = "마나:  " + now_mp.ToString();
    }
    void RefreshLevelStat()
    {
        level_text.text = "Lv:  " + player_stats.GetLevel;
    }
    void RefrestStrStat()
    {
        str_text.text = "공격력:  " + player_stats.Total_Str;
    }

    void RefrestDefStat()
    {
        def_text.text = "방어력:  " + player_stats.Total_Def;
    }
}
