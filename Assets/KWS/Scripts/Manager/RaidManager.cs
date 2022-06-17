using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidManager : MonoBehaviour
{
    public Action<bool> death_callback;

    [Serializable]
    private class SpawnInfo
    {
        public GameObject monster_prefab;
        public Transform[] monster_spawn_position;
    }

    [SerializeField]
    private SpawnInfo[] spawn_infos;

    private int spawn_info_idx;
    private int max_monster;
    private int now_monster;
    private void Start()
    {
        spawn_info_idx = 0;
        death_callback = ReportMonsterDeathCount;

        InitNowMonsterCount();
        SpawnNewMonster();
    }
    private void SpawnNewMonster()
    {

        SpawnInfo spawn_info = spawn_infos[spawn_info_idx];

        for (int i = 0; i < spawn_info.monster_spawn_position.Length; i++)
        {
            GameObject monster = Instantiate(spawn_info.monster_prefab);
            monster.GetComponent<Monster>().Init(death_callback);
            monster.transform.position = spawn_info.monster_spawn_position[i].position;
        }
    }

    void InitNowMonsterCount()
    {
        max_monster = spawn_infos[spawn_info_idx].monster_spawn_position.Length;
        now_monster = 0;
    }

    public void ReportMonsterDeathCount(bool is_main_boss)
    {
        now_monster++;

        if(is_main_boss == true)
        {
            ClearRaid();
        }
        else if(now_monster == max_monster) // Next Monster Spawn
        {
            ClearSection();
        }
    }

    void ClearSection()
    {
        if (spawn_info_idx == spawn_infos.Length - 1) return;
        spawn_info_idx = (++spawn_info_idx % spawn_infos.Length);

        InitNowMonsterCount();
        SpawnNewMonster();
    }

    void ClearRaid()
    {
        Debug.Log("레이드 클리어.");
        StartCoroutine(SlowMotion());
    }

    IEnumerator SlowMotion()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Camera.main.fieldOfView -= 20f;

        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Camera.main.fieldOfView += 20f;
    }
}
