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

        //weapon.SetWeaponTotalDamage(unit_stats.Str);
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

    public void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        if (IsDead() == true) return;
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true) return;
        
        ApplyDefense(ref damage);
        DamagedAnimation();
        StartCoroutine(ActiveHitEffect());
        StartCoroutine(ShowHitParticle(hit_pos));
        StartCoroutine(ShowDamageText(damage));
        LoseHp(damage);
        //DrawHpUI();
        DrawBarUI(unit_hp_ui_group, unit_hp_ui, unit_now_hp, unit_stats.Total_Hp);
        if (IsDead() == true)
            Dye();
    }

    void ApplyDefense(ref float damage)
    {
        damage = (damage - unit_stats.Total_Def);
        if (damage <= 0) damage = 0;
    }

    void DamagedAnimation()
    {
        // 해당 애니메이션이 있다는 조건 있어야 함.
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
    IEnumerator ShowDamageText(float damage)
    {
        //Text text = ObjectPoolingManager.GetDamageText();
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
        yield return new WaitForSeconds(0.03f);
        renderer.material.color = origin_color;
    }
    void LoseHp(float damage)
    {
        unit_now_hp -= damage;
    }
    void DrawHpUI()
    {
        if (unit_hp_ui_group == null) return;
        if (unit_hp_ui_group.activeSelf == false) return;

        float ratio = unit_now_hp / unit_stats.Total_Hp;
        float width = unit_hp_ui.GetComponent<RectTransform>().rect.width;

        unit_hp_ui.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }

    void DrawBarUI(GameObject ui_group, Image ui, float now_value, float total_value)
    {
        if (ui_group == null) return;
        if (ui_group.activeSelf == false) return;

        float ratio = now_value / total_value;
        float width = ui.GetComponent<RectTransform>().rect.width;

        ui.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }
    void DrawMpUI()
    {
        if (unit_mp_ui_group == null) return;
        if (unit_mp_ui_group.activeSelf == false) return;

        float ratio = unit_stats.Total_Mp / initial_mp;
        float width = unit_mp_ui.GetComponent<RectTransform>().rect.width;

        unit_mp_ui.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, (width - width * ratio), 0f);
    }

    bool IsDead() // 반납 하기전에 이펙트 효과 제자리로..
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

    void CancelDamagedAnimation()
    {
        unit_animation.SetBool("IsDamaged", false);

        // 공격 모션중 히트로 모션이 캔슬 될 때, 해당 공격의 후반 프레임에서 ActiveOffWeaponArea를 호출하지 못할 수도 있어서 수동으로 호출
        ActiveOffWeaponArea();
    }
    protected virtual void Dye() { }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") == true || collision.gameObject.CompareTag("Monster") == true)
        {
            Debug.Log("가속도 제로");
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
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
        //DrawMpUI();
    }
}
