using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private UnitStats player_stats;

    #region Ω∫≈» ¡§∫∏
    [SerializeField] private Text level_text;
    [SerializeField] private Text hp_text;
    [SerializeField] private Text mp_text;
    [SerializeField] private Text str_text;
    [SerializeField] private Text def_text;
    #endregion

    private void Start()
    {
        AddCallback();
        RefreshAllStats();
    }

    void AddCallback()
    {
        player_stats.On_Level_Up += RefreshLevelStat;
        player_stats.On_Stats_Change += RefreshAllStats;
    }

    void RefreshAllStats()
    {
        int now_hp = player_stats.Total_Hp;
        int now_mp = player_stats.Total_Mp;

        RefreshLevelStat();

        hp_text.text = now_hp.ToString();
        mp_text.text = now_mp.ToString();

        str_text.text = player_stats.Total_Str.ToString();
        def_text.text = player_stats.Total_Def.ToString();
    }
    void RefreshLevelStat()
    {
        level_text.text = player_stats.GetLevel.ToString();
    }
}
