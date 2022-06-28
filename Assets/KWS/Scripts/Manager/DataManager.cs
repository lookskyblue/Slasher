using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private StatsTableList stats_tables; // 레벨에 따른 플레이어 스탯 테이블
    private PlayerData player_data; // 현재 플레이어의 정보들을 가지는 데이터
    private InventoryData inventory_data;
    private void Awake()
    {
        // StatsTables
        InitStatsTableList();
        SaveStatsTableToJson();
        LoadStatsTableFromJson();

        // PlayerData
        if (IsExistPlayerData() == false)
        {
            InitPlayerData();
            SavePlayerDataToJson(player_data);
        }
        else
        {
            player_data = LoadPlayerDataFromJson();
        }

        // Inventory
        if(IsExistInventoryData() == false)
        {
            inventory_data = new InventoryData();
            SaveInventory(inventory_data);
        }

        else
        {
            inventory_data = LoadInventory();
        }
    }

    bool IsExistInventoryData()
    {
        string path = Path.Combine(Application.dataPath, "inventoryData.json");

        return File.Exists(path);
    }
    public void SaveInventory(InventoryData inventory_data)
    {
        string path = Path.Combine(Application.dataPath, "inventoryData.json");
        string json = JsonUtility.ToJson(inventory_data, true);

        File.WriteAllText(path, json);
    }
    public InventoryData LoadInventory()
    {
        string path = Path.Combine(Application.dataPath, "inventoryData.json");
        string json = File.ReadAllText(path);

        InventoryData inventory_data = JsonUtility.FromJson<InventoryData>(json);

        return inventory_data;
    }
    bool IsExistPlayerData()
    {
        string path = Path.Combine(Application.dataPath, "playerData.json");

        return File.Exists(path);
    }
    public void LoadStatsTableFromJson()
    {
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
        string json_data = JsonUtility.ToJson(player_data, true);
        string path = Path.Combine(Application.dataPath, "playerData.json");
        File.WriteAllText(path, json_data);
    }

    public PlayerData LoadPlayerDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "playerData.json");
        string json_data = File.ReadAllText(path);

        player_data = JsonUtility.FromJson<PlayerData>(json_data);

        return player_data;
    }

    public StatsTable GetStatsTableMatchLevel(int now_level)
    {
        StatsTable stats_table = stats_tables.stats_table_list[now_level - 1].DeepCopy();
        //StatsTable stats_table = stats_tables.stats_table_list[now_level - 1];

        return stats_table;
    }

    public void UpdatePlayerDataInfo(StatsTable stats_table, int now_level, int now_exp, int now_gold)
    {
        player_data.stat_table = stats_table;
        player_data.level = now_level;
        player_data.exp = now_exp;
        player_data.gold = now_gold;
        
        SavePlayerDataToJson(player_data);
    }
    void InitStatsTableList()
    {
        stats_tables = new StatsTableList();

        for (int i = 0; i < 30; i++)
        {
            StatsTable stats_table = new StatsTable();

            stats_table.str = 10;
            stats_table.def = 10;
            stats_table.hp = 200;
            stats_table.mp = 200;
            stats_table.hp_auto_recover_amount = 5;
            stats_table.mp_auto_recover_amount = 5;
            stats_table.max_exp = Mathf.Pow((i * 50 / 49), 2.5f) * 10;

            stats_tables.stats_table_list.Add(stats_table);
        }
    }
    void SaveStatsTableToJson()
    {
        string json_data = JsonUtility.ToJson(stats_tables, true);
        string path = Path.Combine(Application.dataPath, "statsTable.json");
        File.WriteAllText(path, json_data);
    }
}
