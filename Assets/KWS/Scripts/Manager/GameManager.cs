using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player_prefab;
    [SerializeField]
    private GameObject first_spawn_zone;
    [SerializeField]
    private GameObject player_group;
    private Player player;
    public static GameManager instance = null;
    [SerializeField]
    private RaidBoardManager raid_board_manager;
    private Animator palyer_ani;

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

    public MonoBehaviour BorrowMono() { return this; }

    public void SendCompensation(float gold = 0, float exp = 0)
    {
        Debug.Log("보상을 받음 골드: " + gold + ", 경험치: " + exp);

        player.OnChangeExp(exp);
    }

    public RaidBoardManager GetRaidBoardManager() { return raid_board_manager; }
}
