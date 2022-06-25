using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject first_spawn_zone;
    [SerializeField] private GameObject player_group;
    [SerializeField] private RaidBoardManager raid_board_manager;
    [SerializeField] private DragAndDropContainer drag_and_drop_container;

    [Tooltip("아이템을 되팔기 할 때의 정가에서 곱해지는 비율입니다.(0 ~ 1)")]
    [SerializeField] private float resale_ratio;

    private Player player;
    private Animator palyer_ani;
    private bool is_using_store = false;
    private bool is_talking_with_npc = false;

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
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //player = CreatePlayer();
        SetPlayerToSpawnPoint();
        DontDestroyOnLoad(player_group);
        
        player = player_group.transform.GetChild(0).GetComponent<Player>();
        palyer_ani = player.GetComponent<Animator>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    GameObject CreatePlayer()
    {
        return Instantiate(player_prefab);
    }

    public void LoadScene(string next_scene_name) // 비동기로 할 것
    {
        SceneManager.LoadScene(next_scene_name);

        PlayerInit();
    }

    void PlayerInit()
    {
        player.InitUnitStats();
        player.DrawGaugeUI();
        palyer_ani.Play("Idle");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode load_scene_mode)
    {
        SetPlayerToSpawnPoint();
    }

    void SetPlayerToSpawnPoint()
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
}
