using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Unit
{
    [System.Serializable]
    struct AudioData
    {
        public string name;
        public AudioClip sound_clip;
    }

    [SerializeField] private float attack_delay;
    [SerializeField] private float rotate_fower;
    [SerializeField] private float player_find_range;
    [SerializeField] private List<int> attack_phase_list;
    [SerializeField] private AudioData[] audio_data;
    private Dictionary<string, AudioClip> audio_dic = new Dictionary<string, AudioClip>();
    private NavMeshAgent nav_mesh_agent;
    private Coroutine is_doing_attack_motion_cor = null;
    private AudioSource audio_source;
    private System.Random rand;

    protected Action<bool> death_callback;
    void Awake()
    {
        audio_source = GetComponent<AudioSource>();

        for(int i = 0; i < audio_data.Length; i++)
        {
            audio_dic.Add(audio_data[i].name, audio_data[i].sound_clip);
        }
    }
    protected void Start()
    {
        base.Start();
        InitUnitStats();

        nav_mesh_agent = GetComponent<NavMeshAgent>();
        nav_mesh_agent.enabled = true;

        InstancingMaterial();

        rand = new System.Random();
    }

    public void InitUnitStats()
    {
        unit_stats.Total_Hp = unit_stats.Default_Hp;
        unit_stats.Total_Mp = unit_stats.Default_Mp;
        initial_mp = unit_stats.Total_Mp;
        unit_stats.Total_Str = unit_stats.Default_Str;
        unit_stats.Total_Def = unit_stats.Default_Def;

        unit_now_hp = unit_stats.Total_Hp;
    }

    public void Init(Action<bool> death_callback)
    {
        this.death_callback = death_callback;
    }

    public override void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        base.ReceiveDamage(damage, hit_pos);

        DamagedAnimation();
    }

    public override void DamagedAnimation()
    {
        if (IsDead() == false)
            base.DamagedAnimation();
    }
    protected override void Die()
    {
        GetComponent<TriggerCallback>().collision_stay_event = null;
        nav_mesh_agent.enabled = false;
        unit_animation.Play("Dying");
        EffectReset();
        ActiveOffWeaponArea(0);
        ActiveOffWeaponArea(1);
        ReportDeath();
        DropItem();
        StartCoroutine(DestroyObject());
    }

    void DropItem()
    {
        if (rand.Next(0, 2) == 0) return; // 1?? ?? ?????? ????. 1/2 ????

        ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("RandomItem").transform.position = transform.position;
    }

    void InstancingMaterial()
    {
        renderer.material = Instantiate(renderer.material);
        origin_color = renderer.material.color;
    }
    public virtual void ReportDeath()
    {
        death_callback(false);
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(3f); // ???? ???? ?????????? 3?? ???? ???? ???? ???????? ???? ?????????? ?????? ???????? ???? ????.
        Destroy(gameObject);
    }
    void EffectReset()
    {
        renderer.material.color = origin_color;
    }
    void FollowingPlayer()
    {
        if (nav_mesh_agent.enabled == false) return;

        float dist = Vector3.Distance(ObjectPoolingManager.Instance.Player_Transform.position, transform.position);

        if (dist < player_find_range) // ???? ????
        {
            nav_mesh_agent.isStopped = false;

            if (IsDoingAnotherMotion() == true)
            {
                nav_mesh_agent.isStopped = true;
                //unit_animation.SetBool("IsRun", false);
                return;
            }

            if (dist <= nav_mesh_agent.stoppingDistance) // ???? ????
            {
                if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Run") == true)
                    unit_animation.SetBool("IsRun", false);

                if (is_doing_attack_motion_cor == null)
                    is_doing_attack_motion_cor = StartCoroutine(OnAttackMotion());
            }

            else
            {
                if(IsDoingAnotherMotion() == true)
                {
                    nav_mesh_agent.isStopped = true;
                    //unit_animation.SetBool("IsRun", false);
                    return;
                }

                if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
                    unit_animation.SetBool("IsRun", true);

                nav_mesh_agent.SetDestination(ObjectPoolingManager.Instance.Player_Transform.position);

                Vector3 dir = ObjectPoolingManager.Instance.Player_Transform.position - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotate_fower);
            }
        }

        else // ???? ???? ??
        {
            unit_animation.SetBool("IsRun", false);
            unit_animation.SetInteger("AttackPhase", 0);

            nav_mesh_agent.isStopped = true;
        }
    }

    bool IsDoingAnotherMotion()
    {
        bool result = false;

        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true ||
            unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == true ||
            unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack2") == true ||
            unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack3") == true ||
            unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack4") == true)
        {
            result = true;
        }

        return result;
    }

    private void Update()
    {
        FollowingPlayer();
    }
    IEnumerator OnAttackMotion()
    {
        float total_time = 0f;

        while (total_time < attack_delay)
        {
            total_time += Time.deltaTime;
            yield return null;
        }

        int attack_motion_idx = GetRandomAttackMotionIdx();

        unit_animation.SetInteger("AttackPhase", attack_motion_idx);

        yield return new WaitForSeconds(0.2f);

        is_doing_attack_motion_cor = null;
    }

    int GetRandomAttackMotionIdx()
    {
        int rand_idx = UnityEngine.Random.Range(0, attack_phase_list.Count);
        int rand_attack_phase = attack_phase_list[rand_idx];

        return rand_attack_phase;
    }

    private void OffAttackMotion()
    {
        unit_animation.SetInteger("AttackPhase", 0);
    }
    protected IEnumerator LookAtPlayer()
    {
        float total_time = 0f;
        Vector3 dir = ObjectPoolingManager.Instance.Player_Transform.position - transform.position;

        while (total_time < 1)
        {
            yield return null;
            total_time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), total_time);
        }
    }

    void PlaySound(string sound_name)
    {
        if(audio_dic.ContainsKey(sound_name) == false)
        {
            Debug.LogError("Does not contains sound name: " + sound_name);

            return;
        }

        audio_source.PlayOneShot(audio_dic[sound_name]);
    }
}
