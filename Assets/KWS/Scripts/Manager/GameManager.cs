using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameObject player_group;
    [SerializeField] private RaidBoardManager raid_board_manager;
    [SerializeField] private DragAndDropContainer drag_and_drop_container;

    [Tooltip("아이템을 되팔기 할 때의 정가에서 곱해지는 비율입니다.(0 ~ 1)")]
    [SerializeField] private float resale_ratio;

    private Player player;
    private DataManager data_manager;

    private bool is_using_store = false;
    private bool is_talking_with_npc = false;
    private bool is_destroy = false;
    public RaidBoardManager GetRaidBoardManager { get { return raid_board_manager; } }
    public DragAndDropContainer GetDragAndDropContainer { get { return drag_and_drop_container; } }
    public float GetResaleRatio { get { return resale_ratio; } }
    public MonoBehaviour BorrowMono { get { return this; } }

    public bool Is_Using_Store 
    {
        get { return is_using_store; } 
        set { is_using_store = value; } 
    }

    public bool Is_Talking_With_Npc
    {
        get { return is_talking_with_npc; }
        set { is_talking_with_npc = value; }
    }

    private void Awake()
    {
        if (instance == null) // 첫 씬일 때는 일단 생성. 
        {
            instance = this;

            player = player_group.transform.GetChild(0).GetComponent<Player>();
            data_manager = player.GetComponent<DataManager>();

            SetPlayerToSpawnPoint(player_group);
            DontDestroyOnLoad(player_group);
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            is_destroy = true;
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (is_destroy == true) return;

        SetPlayerData();

        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void SetPlayerData()
    {
        PlayerData player_data = data_manager.LoadPlayerDataFromJson();
        InventoryData inventory_data = data_manager.LoadInventory();

        player.Init(player_data);
        InventoryManager.instance.AddItem(inventory_data);
    }

    public void LoadScene(string next_scene_name) // 비동기로 할 것
    {
        SceneManager.LoadScene(next_scene_name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode load_scene_mode)
    {
        SetPlayerToSpawnPoint(player_group);
        player.SetIdleMotion();
        player.FillUpStamina();
    }

    void SetPlayerToSpawnPoint(GameObject player_group)
    {
        GameObject player_spawn_position = GameObject.Find("PlayerSpawnPosition");

        player_group.transform.GetChild(0).position = player_spawn_position.transform.position;
        player_group.transform.GetChild(0).rotation = player_spawn_position.transform.rotation;
        player_group.transform.GetChild(1).rotation = player_group.transform.GetChild(0).rotation;
    }

    public void SendCompensation(float gold = 0, float exp = 0)
    {
        Debug.Log("보상을 받음 골드: " + gold + ", 경험치: " + exp);

        player.OnChangeExp(exp);
    }
    public bool IsDoingOtherTask()
    {
        bool is_doing_task = false;

        if (is_using_store == true || is_talking_with_npc == true)
            is_doing_task = true;

        return is_doing_task;
    }

    public void ApplyStatsAccordingToLevel(int now_level)
    {
        Debug.Log("Apply Stats AccordingToLevel");

        // Get New Data
        StatsTable stats_table = data_manager.GetStatsTableMatchLevel(now_level);
        int level = now_level;
        int exp = 0;
        int gold = InventoryManager.instance.Gold_On_Hand;

        // Save
        data_manager.UpdatePlayerDataInfo(stats_table, level, exp, gold);
        
        // Load
        PlayerData player_data = data_manager.LoadPlayerDataFromJson();

        // Apply To Game
        player.ApplyPlayerDataToGame(player_data);
        player.DrawUI();
    }

    void OnApplicationQuit()
    {
        SaveNowPlayerData();
    }

    public void SaveNowPlayerData()
    {
        Debug.Log("현재 정보 저장");
        // 어쩌피 처음 씬은 마을이므로 기본 스텟은 별도로 저장하지 않음. 레벨에 맞는 스텟만 저장

        PlayerData player_data = data_manager.LoadPlayerDataFromJson();

        player_data.gold = InventoryManager.instance.Gold_On_Hand;
        player_data.exp = player.GetNowExp();

        data_manager.SavePlayerDataToJson(player_data);
    }
}
