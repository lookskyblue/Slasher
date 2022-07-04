using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    struct Compensation
    {
        public int exp;
        public int gold;
    }
    
    [SerializeField] private GameObject player_group;
    [SerializeField] private RaidBoardManager raid_board_manager;
    [SerializeField] private DragAndDropContainer drag_and_drop_container;
    [SerializeField] private SkillBookManager skill_book_manager;

    [Tooltip("아이템을 되팔기 할 때의 정가에서 곱해지는 비율입니다.(0 ~ 1)")]
    [SerializeField] private float resale_ratio;

    private Player player;
    private DataManager data_manager;
    private Queue<Compensation> compensation_queue = new Queue<Compensation>();
    public static GameManager instance = null;

    private bool is_using_store = false;
    private bool is_talking_with_npc = false;
    private bool is_destroy = false;
    private bool is_doing_raid = true;
    private bool is_doing_loading_page = false;
    public bool Is_Doing_Raid { get { return is_doing_raid; } }
    public bool Is_Doing_Loading_Page { get { return is_doing_loading_page; } }
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
        BGMManager.instance.PlayBgm(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Confined;
        
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void SetPlayerData()
    {
        PlayerData player_data = data_manager.LoadData<PlayerData>(data_manager.Player_Data_File_Name);
        player.Init(player_data);

        InventoryData inventory_data = data_manager.LoadData<InventoryData>(data_manager.Inventory_Data_File_Name);
        InventoryManager.instance.AddItem(inventory_data);
        
        SkillDataList skill_data = data_manager.LoadData<SkillDataList>(data_manager.Skill_Data_file_name);
        skill_book_manager.SetData(skill_data);
    }
    public void LoadScene(string next_scene_name) // 비동기로 할 것
    {
        is_doing_loading_page = true;

        BGMManager.instance.StopBgm();
        ObjectPoolingManager.Instance.WithdrawAllObject();
        SceneLoader.Instance.LoadScene(next_scene_name);
    }
    public void ReportEndOfSceneLoad(Scene scene)
    {
        is_doing_raid = scene.name.Equals("Town") ? false : true;

        BGMManager.instance.PlayBgm(scene.name);

        SetPlayerToSpawnPoint(player_group);
        CheckCompensation();
        player.SetIdleMotion();
        player.FillUpStamina();

        is_doing_loading_page = false;
    }
    void CheckCompensation()
    {
        while(compensation_queue.Count != 0)
        {
            Compensation comp = compensation_queue.Dequeue();

            player.OnChangeExp(comp.exp);
            InventoryManager.instance.Gold_On_Hand += comp.gold;
        }
    }
    void SetPlayerToSpawnPoint(GameObject player_group)
    {
        GameObject player_spawn_position = GameObject.Find("PlayerSpawnPosition");

        player_group.transform.GetChild(0).position = player_spawn_position.transform.position;
        player_group.transform.GetChild(0).rotation = player_spawn_position.transform.rotation;
        player_group.transform.GetChild(1).rotation = player_group.transform.GetChild(0).rotation;
    }
    public void SendCompensation(int gold = 0, int exp = 0)
    {
        Debug.Log("보상을 받음 골드: " + gold + ", 경험치: " + exp);

        Compensation compensation;
        compensation.gold = gold;
        compensation.exp = exp;

        compensation_queue.Enqueue(compensation);
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
        PlayerData player_data = data_manager.LoadData<PlayerData>(data_manager.Player_Data_File_Name);

        // Apply To Game
        player.ApplyPlayerDataToGame(player_data);
        player.ApplyMountedItemStats();
        player.DrawUI();

        // Load, Update, Save SkillData
        SkillDataList skill_data = data_manager.LoadData<SkillDataList>(data_manager.Skill_Data_file_name);
        
        skill_data.remaining_skill_point++;
        skill_book_manager.SetData(skill_data);
        data_manager.SaveData<SkillDataList>(skill_data, data_manager.Skill_Data_file_name);
    }
    void OnApplicationQuit()
    {
        SaveNowPlayerData();
    }
    public void SaveNowPlayerData()
    {
        Debug.Log("현재 정보 저장");
        // 어쩌피 처음 씬은 마을이므로 기본 스텟은 별도로 저장하지 않음. 레벨에 맞는 스텟만 저장

        PlayerData player_data = data_manager.LoadData<PlayerData>(data_manager.Player_Data_File_Name);

        player_data.gold = InventoryManager.instance.Gold_On_Hand;
        player_data.exp = player.GetNowExp();

        data_manager.SaveData<PlayerData>(player_data, data_manager.Player_Data_File_Name);
    }
    public void PushExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
