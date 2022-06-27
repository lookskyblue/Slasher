using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private StatsTableList stats_tables; // 레벨에 따른 플레이어 스탯 테이블
    private PlayerData player_data; // 현재 플레이어의 정보들을 가지는 데이터
    
    private void Awake()
    {
        //InitStatsTableList();
        //SaveStatsTableToJson();

        InitPlayerData();
        SavePlayerDataToJson(player_data);
        
        LoadStatsTableFromJson();
    }
    public void LoadStatsTableFromJson()
    {
        Debug.Log("StatsTable 데이터를 Json으로부터 불러옵니다.");

        string path = Path.Combine(Application.dataPath, "statsTable.json");
        string json_data = File.ReadAllText(path);

        stats_tables = JsonUtility.FromJson<StatsTableList>(json_data);
    }

    void InitPlayerData()
    {
        player_data = new PlayerData();
        player_data.stat_table = new StatsTable();

        player_data.stat_table.str = 20;
        player_data.stat_table.def = 20;
        player_data.stat_table.hp = 200;
        player_data.stat_table.mp = 200;
        player_data.stat_table.hp_auto_recover_amount = 5;
        player_data.stat_table.mp_auto_recover_amount = 5;
        player_data.stat_table.mp_auto_recover_amount = 5;
        player_data.stat_table.max_exp = 5;

        player_data.level = 1;
        player_data.exp = 0;
        player_data.gold = 123;

        player_data.current_position = ObjectPoolingManager.Instance.Player_Transform.position;
        player_data.current_rotation = ObjectPoolingManager.Instance.Player_Transform.rotation;
    }

    public void SavePlayerDataToJson(PlayerData player_data)
    {
        Debug.Log("데이터를 Json으로 저장합니다.");
        
        string json_data = JsonUtility.ToJson(player_data, true);
        string path = Path.Combine(Application.dataPath, "playerData.json");
        File.WriteAllText(path, json_data);
    }

    public PlayerData LoadPlayerDataFromJson()
    {
        Debug.Log("데이터를 Json으로부터 불러옵니다.");

        string path = Path.Combine(Application.dataPath, "playerData.json");
        string json_data = File.ReadAllText(path);

        player_data = JsonUtility.FromJson<PlayerData>(json_data);

        return player_data;
    }

    public StatsTable GetStatsTableMatchLevel(int now_level)
    {
        //StatsTable stats_table = stats_tables.stats_table_list[now_level - 1].DeepCopy();
        StatsTable stats_table = stats_tables.stats_table_list[now_level - 1];

        return null;
    }

    public void UpdatePlayerDataInfo(StatsTable stats_table, int now_level, int now_exp, int now_gold)
    {
        player_data.stat_table = stats_table;
        player_data.level = now_level;
        player_data.exp = now_exp;
        player_data.gold = now_gold;
        
        SavePlayerDataToJson(player_data);
    }

    //void InitStatsTableList()
    //{
    //    for (int i = 0; i < 30; i++)
    //    {
    //        StatsTable stats_table = new StatsTable();

    //        stats_table.str = 10;
    //        stats_table.def = 10;
    //        stats_table.hp = 200;
    //        stats_table.mp = 200;
    //        stats_table.hp_auto_recover_amount = 5;
    //        stats_table.mp_auto_recover_amount = 5;
    //        stats_table.max_exp = Mathf.Pow((i * 50 / 49), 2.5f) * 10;

    //        stats_table_list.stats_table_list.Add(stats_table);
    //    }

    //    Debug.Log("6랩의 max exp: " + stats_table_list.stats_table_list[5].max_exp);
    //}

    //void SaveStatsTableToJson()
    //{
    //    Debug.Log("StatsTable을 Json으로 저장합니다.");

    //    string json_data = JsonUtility.ToJson(, true);
    //    string path = Path.Combine(Application.dataPath, "statsTable.json");
    //    File.WriteAllText(path, json_data);
    //}
}
