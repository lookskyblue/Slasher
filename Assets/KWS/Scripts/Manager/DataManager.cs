using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private readonly string stats_table_file_name = "statsTable.json";
    private readonly string player_data_file_name = "playerData.json";
    private readonly string inventory_data_file_name = "inventoryData.json";
    private readonly string skill_data_file_name = "skillData.json";
    public string Player_Data_File_Name { get { return player_data_file_name; } }
    public string Inventory_Data_File_Name { get { return inventory_data_file_name; } }
    public string Skill_Data_file_name { get { return skill_data_file_name; } }

    private StatsTableList stats_tables;
    private PlayerData player_data; 
    private InventoryData inventory_data;
    private SkillDataList skill_data;
    private string persistent_data_path;
    private void Awake()
    {
        persistent_data_path = Application.persistentDataPath;

        CheckData<StatsTableList>(ref stats_tables, CreateStatsTableList, stats_table_file_name);
        CheckData<PlayerData>(ref player_data, CreatePlayerData, player_data_file_name);
        CheckData<InventoryData>(ref inventory_data, CreateInventoryData, inventory_data_file_name);
        CheckData<SkillDataList>(ref skill_data, CreateSkillData, skill_data_file_name);
    }
    bool IsExistData(string file_name)
    {
        string path = Path.Combine(persistent_data_path, file_name);

        return File.Exists(path);
    }
    public void SaveData<T>(T data, string file_name)
    {
        string path = Path.Combine(persistent_data_path, file_name);
        string json_data = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json_data);
    }
    public T LoadData<T>(string file_name)
    {
        string path = Path.Combine(persistent_data_path, file_name);
        string json_data = File.ReadAllText(path);

        return JsonUtility.FromJson<T>(json_data);
    }
    void CheckData<T>(ref T data, Action action, string file_name)
    {
        if (IsExistData(file_name) == false)
        {
            action.Invoke();
            SaveData<T>(data, file_name);
        }

        else
        {
            data = LoadData<T>(file_name);
        }
    }
    void CreateStatsTableList()
    {
        stats_tables = new StatsTableList();

        int str_adjust_value = 0;
        int def_adjust_value = 0;
        int hp_adjust_value = 0;
        int mp_adjust_value = 0;

        for (int i = 0; i < 30; i++)
        {
            StatsTable stats_table = new StatsTable();

            stats_table.str = 10 + str_adjust_value;
            stats_table.def = 10 + def_adjust_value;
            stats_table.hp = 200 + hp_adjust_value;
            stats_table.mp = 200 + mp_adjust_value;
            stats_table.hp_auto_recover_amount = 5;
            stats_table.mp_auto_recover_amount = 5;
            stats_table.max_exp = Mathf.Pow((i * 50 / 49), 2.5f) * 10;

            stats_tables.stats_table_list.Add(stats_table);

            str_adjust_value += 5;
            def_adjust_value += 5;
            hp_adjust_value += 50;
            mp_adjust_value += 50;
        }
    }
    void CreatePlayerData()
    {
        player_data = new PlayerData();
        player_data.stat_table = stats_tables.stats_table_list[0];
        player_data.stat_table.max_exp = 5;

        player_data.level = 1;
        player_data.exp = 0;
        player_data.gold = 3000;
    }
    void CreateInventoryData()
    {
        inventory_data = new InventoryData();
    }
    void CreateSkillData()
    {
        skill_data = new SkillDataList();
        skill_data.remaining_skill_point = 5;
    }
    public void UpdatePlayerDataInfo(StatsTable stats_table, int now_level, int now_exp, int now_gold)
    {
        player_data.stat_table = stats_table;
        player_data.level = now_level;
        player_data.exp = now_exp;
        player_data.gold = now_gold;
        
        SaveData<PlayerData>(player_data, player_data_file_name);
    }
    public StatsTable GetStatsTableMatchLevel(int now_level)
    {
        StatsTable stats_table = stats_tables.stats_table_list[now_level - 1].DeepCopy();

        return stats_table;
    }
}
