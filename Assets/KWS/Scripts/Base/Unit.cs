using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KWSWeapon;

public class Unit : MonoBehaviour
{
    [SerializeField]
    protected float unit_now_hp;
    [SerializeField]
    protected Canvas unit_canvas;
    [SerializeField]
    protected GameObject unit_hp_ui_group;
    [SerializeField]
    protected Image unit_hp_ui;
    [SerializeField]
    protected GameObject unit_mp_ui_group;
    [SerializeField]
    protected Image unit_mp_ui;
    [SerializeField]
    protected Renderer renderer;

    [SerializeField]
    protected Color change_color;
    protected Color origin_color;
    protected Animator unit_animation;
    [SerializeField]
    protected Weapon weapon;
    [SerializeField]
    protected UnitStats unit_stats;
    private float initial_mp;
    protected void Start()
    {
        unit_animation = GetComponent<Animator>();

        InitUnitStats();

        origin_color = renderer.material.color;
    }

    void InitUnitStats()
    {
        unit_stats.On_Change_Mp += OnChangeMp;
        unit_stats.AcceptUsedPotion += AcceptUsedPotion;

        unit_stats.Total_Hp  = unit_stats.Default_Hp;
        unit_stats.Total_Mp  = unit_stats.Default_Mp;
        initial_mp = unit_stats.Total_Mp;
        unit_stats.Total_Str = unit_stats.Default_Str;
        unit_stats.Total_Def = unit_stats.Default_Def;

        unit_now_hp = unit_stats.Total_Hp;
    }

    public virtual void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        if (IsDead() == true) return;
        
        ApplyDefense(ref damage);
        StartCoroutine(ActiveHitEffect());
        StartCoroutine(ShowHitParticle(hit_pos));
        StartCoroutine(ShowDamageText(damage));
        LoseHp(damage);
        DrawBarUI(unit_hp_ui_group, unit_hp_ui, unit_now_hp, unit_stats.Total_Hp);
        if (IsDead() == true)
            Dye();
    }

    void ApplyDefense(ref float damage)
    {
        damage = (damage - unit_stats.Total_Def);
        if (damage <= 0) damage = 0;
    }

    public virtual void DamagedAnimation()
    {
        unit_animation.SetBool("IsDamaged", true);
    }

    IEnumerator ShowHitParticle(Vector3 hit_pos)
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("HitParticle");
        ParticleSystem hit_particle = obj.GetComponent<ParticleSystem>();

        hit_particle.transform.position = hit_pos;
        hit_particle.Play();

        yield return new WaitForSeconds(0.5f);

        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("HitParticle", obj);
    }
    public virtual IEnumerator ShowDamageText(float damage)
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("DamageCanvas");
        Text text = obj.transform.GetChild(0).GetComponent<Text>();

        int i_damage = (int)damage;

        text.text = i_damage.ToString();
        obj.transform.SetParent(transform);

        ObjectPoolingManager.Instance.InitDamageTextTransform(ref obj);

        yield return new WaitForSeconds(1f);

        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("DamageCanvas", obj);
    }

    IEnumerator ActiveHitEffect()
    {
        renderer.material.color = change_color;
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = origin_color;
    }
    void LoseHp(float damage)
    {
        unit_now_hp -= damage;
    }
    void DrawBarUI(GameObject ui_group, Image ui, float now_value, float total_value)
    {
        if (ui_group == null) return;
        if (ui_group.activeSelf == false) return;

        float ratio = now_value / total_value;
        float width = ui.GetComponent<RectTransform>().rect.width;

        ui.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }
    protected bool IsDead() // 반납 하기전에 이펙트 효과 제자리로..
    {
        return unit_now_hp <= 0;
    }
    void ActiveOnWeaponArea()
    {
        weapon.ActiveOnWeaponArea();
    }

    protected void ActiveOffWeaponArea()
    {
        weapon.ActiveOffWeaponArea();
    }

    IEnumerator CancelDamagedAnimation()
    {
        ActiveOffWeaponArea();

        while (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true)
        {
            unit_animation.SetBool("IsDamaged", false);
            yield return null;
        }
    }
    protected virtual void Dye() { }
    
    private void AcceptUsedPotion(int hp, int mp)
    {
        unit_now_hp += hp;
        unit_stats.SetValidHpRange(ref unit_now_hp);
        DrawBarUI(unit_hp_ui_group, unit_hp_ui, unit_now_hp, unit_stats.Total_Hp);

        unit_stats.Total_Mp += mp;
        DrawBarUI(unit_mp_ui_group, unit_mp_ui, unit_stats.Total_Mp, initial_mp);
    }

    private void OnChangeMp(int mp)
    {
        unit_stats.Total_Mp += mp;
        DrawBarUI(unit_mp_ui_group, unit_mp_ui, unit_stats.Total_Mp, initial_mp);
    }
}
