using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit
{
    private static Player player = null;

    [SerializeField] private Image exp_ui_image;
    [SerializeField] private Text exp_text;
    [SerializeField] private Text overhead_level_text;
    [SerializeField] private Text overhead_name_text;
    [SerializeField] private int auto_recovery_wait_time;

    private bool is_damaged = false;
    private bool is_destroy = false;
    private void Awake()
    {
        if (player != null)
        {
            is_destroy = true;
            Destroy(transform.parent.gameObject);
        }
        else 
        {
            player = this;
        }
    }
    public void ApplyPlayerDataToGame(PlayerData player_data)
    {
        if (is_destroy == true) return;

        unit_stats.LevelUp = player_data.level;
        unit_stats.Total_Hp = player_data.stat_table.hp;
        unit_stats.Total_Mp = player_data.stat_table.mp;
        unit_stats.Total_Str = player_data.stat_table.str;
        unit_stats.Total_Def = player_data.stat_table.def;
        unit_stats.Hp_Auto_Recovery_Amount = player_data.stat_table.hp_auto_recover_amount;
        unit_stats.Mp_Auto_Recovery_Amount = player_data.stat_table.mp_auto_recover_amount;
        unit_stats.MaxExp = player_data.stat_table.max_exp;
        unit_stats.Total_Exp = player_data.exp;

        InventoryManager.instance.Gold_On_Hand = player_data.gold;

        //player.transform.position = player_data.current_position;
        //player.transform.rotation = player_data.current_rotation;

        initial_mp = unit_stats.Total_Mp;
        unit_now_hp = unit_stats.Total_Hp;
    }
    public void Init(PlayerData player_data)
    {
        base.Start();
        
        ApplyPlayerDataToGame(player_data);
        AddCallback();
        ApplyMountedItemStats(); // 나중에 장비에 체력도 추가한다면 이 함수가 ApplyPlayerDataInGame함수보다 먼저 호출되거나 해야함
        DrawUI();
        
        StartCoroutine(AutoRecoverHpAndMp());
    }

    public void DrawUI()
    {
        DrawGaugeUI();
        DrawLevelUI();
        DrawNameUI();
    }

    public void SetIdleMotion() { unit_animation.Play("Idle"); }

    public void FillUpStamina()
    {
        unit_now_hp = unit_stats.Total_Hp;
        unit_stats.Total_Mp = initial_mp;

        DrawGaugeUI();
    }

    IEnumerator AutoRecoverHpAndMp()
    {
        int hp_auto_recovery_amount = unit_stats.Hp_Auto_Recovery_Amount;
        int mp_auto_recovery_amount = unit_stats.Mp_Auto_Recovery_Amount;

        while(true)
        {
            if(IsDead() == false)
            {
                if(unit_now_hp < unit_stats.Total_Hp)
                {
                    unit_now_hp = Mathf.Clamp(unit_now_hp + hp_auto_recovery_amount, 0, unit_stats.Total_Hp);
                }

                if(unit_stats.Total_Mp < initial_mp)
                {
                    unit_stats.Total_Mp = Mathf.Clamp(unit_stats.Total_Mp + mp_auto_recovery_amount, 0, initial_mp);
                }

                DrawGaugeUI();
            }

            yield return new WaitForSeconds(auto_recovery_wait_time);
        }
    }

    public void ApplyMountedItemStats()
    {
        InventoryManager.instance.ApplyAllMountedItemStats();
    }

    public void DrawGaugeUI()
    {
        DrawExpUI();
        DrawBarUI(unit_hp_ui_group, unit_hp_ui, unit_now_hp, unit_stats.Total_Hp);
        DrawBarUI(unit_mp_ui_group, unit_mp_ui, unit_stats.Total_Mp, initial_mp);
    }

    public override void DamagedAnimation()
    {
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Skill1") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("GroundCrack") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Smash") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("CannonShot") == true)
        {
            is_damaged = false;
            return;
        }

        is_damaged = true;
        base.DamagedAnimation();
    }

    public override void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        if (IsDead() == true) return;
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true) return;
        if (is_damaged == true) return;

        base.ReceiveDamage(damage, hit_pos);

        if (IsDead() == false) DamagedAnimation();
        else ReportDeathToRaidManager();
    }

    void ReportDeathToRaidManager()
    {
        RaidManager raid_manager = GameObject.FindObjectOfType<RaidManager>();

        if(raid_manager == null)
        {
            Debug.LogError("raid manager is null");
            
            return;
        }

        raid_manager.ReportPlayerDeath();
    }

    public override IEnumerator ShowDamageText(float damage)
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("DamageCanvasPlayer");
        Text text = obj.transform.GetChild(0).GetComponent<Text>();

        int i_damage = (int)damage;

        text.text = i_damage.ToString();
        obj.transform.SetParent(transform);

        ObjectPoolingManager.Instance.InitDamageTextTransform(ref obj);

        yield return new WaitForSeconds(1f);

        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("DamageCanvasPlayer", obj);
    }

    void EndGetDamageMotion()
    {
        is_damaged = false;
    }

    public void OnChangeExp(float acquired_exp) // 게임 매니저에서 호출할 함수이다.
    {
        while(true)
        {
            if(IsLevelUp(acquired_exp, out float left_exp) == true)
            {
                Debug.Log("축하합니다. 레벨업 했습니다.");

                unit_stats.LevelUp = unit_stats.GetLevel + 1;
                //unit_stats.Total_Exp = 0;
                //unit_stats.MaxExp = Mathf.Pow(((unit_stats.GetLevel - 1) * 50 / 49), 2.5f) * 10;

                GameManager.instance.ApplyStatsAccordingToLevel(unit_stats.GetLevel);

                // 사운드 및 파티클 이펙트
                DrawLevelUI();
                DrawGaugeUI();
                StartCoroutine(ShowLevelUpParticle());

                acquired_exp = left_exp;
            }

            else
            {
                unit_stats.Total_Exp = unit_stats.Total_Exp + acquired_exp;
                break;
            }

        }

        DrawExpUI();
    }
    bool IsLevelUp(float acquired_exp, out float left_exp)
    {
        bool is_level_up = false;
        left_exp = 0;

        float after_exp = unit_stats.Total_Exp + acquired_exp;

        if (unit_stats.MaxExp <= after_exp) // 초과업
        {
            is_level_up = true;
            left_exp = after_exp - unit_stats.MaxExp;
        }

        return is_level_up;
    }

    void DrawExpUI()
    {
        float now_exp = unit_stats.Total_Exp;
        float max_exp = unit_stats.MaxExp;

        //exp_text.text = "EXP:  " + (int)now_exp + " / " + (int)max_exp; 절대량 표기 
        exp_text.text = "EXP:  " + (Math.Truncate((now_exp / max_exp) * 100) / 100) * 100 + "%"; // 비율 표기 

        float width = exp_ui_image.GetComponent<RectTransform>().rect.width;
        float ratio = now_exp / max_exp;

        exp_ui_image.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }

    void DrawNameUI()
    {
        overhead_name_text.text = "캐라니스";
    }

    void DrawLevelUI()
    {
        overhead_level_text.text = "Lv." + unit_stats.GetLevel;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnChangeExp(1);
        }
    }

    IEnumerator ShowLevelUpParticle()
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("LevelUp");

        ParticleSystem particle = obj.GetComponent<ParticleSystem>();

        particle.transform.SetParent(transform);
        particle.transform.position = transform.position;
        particle.Play();

        yield return new WaitForSeconds(2f);

        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("LevelUp", obj);
    }

    protected override void Dye()
    {
        weapon.ActiveOffWeaponArea(0);
        unit_animation.Play("Dying");
    }

    public int GetNowExp() { return (int)unit_stats.Total_Exp; }
}
