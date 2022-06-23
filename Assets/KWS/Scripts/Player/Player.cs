using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit
{
    private static Player player = null;

    [SerializeField]
    private Image exp_ui_image;
    [SerializeField]
    private Text exp_text;
    [SerializeField]
    private Text overhead_level_text;
    [SerializeField]
    private Text overhead_name_text;

    private bool is_damaged = false;

    private void Awake()
    {
        if (player != null) Destroy(transform.parent.gameObject);
        else player = this;
    }
    private void Start()
    {
        base.Start();

        AddCallback();
        DrawGaugeUI();
        DrawLevelUI();
        DrawNameUI();
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

    public void OnChangeExp(float acquired_exp) // ���� �Ŵ������� ȣ���� �Լ��̴�.
    {
        while(true)
        {
            if(IsLevelUp(acquired_exp, out float left_exp) == true)
            {
                Debug.Log("�����մϴ�. ������ �߽��ϴ�.");

                unit_stats.LevelUp = unit_stats.GetLevel + 1;
                unit_stats.Total_Exp = 0;
                unit_stats.MaxExp = Mathf.Pow(((unit_stats.GetLevel - 1) * 50 / 49), 2.5f) * 10;

                //unit_stats.Default_Str = 2;

                // ���� �� ��ƼŬ ����Ʈ
                GrowUpStats();
                DrawLevelUI();
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

        if (unit_stats.MaxExp <= after_exp) // �ʰ���
        {
            is_level_up = true;
            left_exp = after_exp - unit_stats.MaxExp;
        }

        return is_level_up;
    }

    void GrowUpStats()
    {
        unit_stats.Default_Hp += 100;
        unit_stats.Total_Hp += 100;

        unit_stats.Default_Mp += 50;
        unit_stats.Total_Mp += 50;

        //DrawBarUI(unit_hp_ui_group, unit_hp_ui, unit_now_hp, unit_stats.Total_Hp);
        //DrawBarUI(unit_mp_ui_group, unit_mp_ui, unit_now_m, unit_stats.Total_Mp);
    }

    void DrawExpUI()
    {
        float now_exp = unit_stats.Total_Exp;
        float max_exp = unit_stats.MaxExp;

        //exp_text.text = "EXP:  " + (int)now_exp + " / " + (int)max_exp; ���뷮 ǥ�� 
        exp_text.text = "EXP:  " + (Math.Truncate((now_exp / max_exp) * 100) / 100) * 100 + "%"; // ���� ǥ�� 

        float width = exp_ui_image.GetComponent<RectTransform>().rect.width;
        float ratio = now_exp / max_exp;

        exp_ui_image.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }

    void DrawNameUI()
    {
        overhead_name_text.text = "ĳ��Ͻ�";
    }

    void DrawLevelUI()
    {
        overhead_level_text.text = "Lv." + unit_stats.GetLevel;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnChangeExp(unit_stats.TestExp);
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
        weapon.ActiveOffWeaponArea();
        unit_animation.Play("Dying");
    }
}
