using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Unit
{
    [SerializeField]
    private float attack_delay;
    [SerializeField]
    private float rotate_fower;
    private NavMeshAgent nav_mesh_agent;
    private Coroutine is_doing_attack_motion_cor = null;
    [SerializeField]
    private float player_find_range;

    protected Action<bool> death_callback;

    private void Start()
    {
        base.Start();

        nav_mesh_agent = GetComponent<NavMeshAgent>();
        nav_mesh_agent.enabled = true;

        InstancingMaterial();
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
    protected override void Dye()
    {
        GetComponent<TriggerCallback>().collision_stay_event = null;
        nav_mesh_agent.enabled = false;
        unit_animation.Play("Dying");
        EffectReset();
        ActiveOffWeaponArea();
        ReportDeath();
        StartCoroutine(DestroyObject());
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
        yield return new WaitForSeconds(3f); // 다른 풀링 이펙트들은 3초 보다 훨씬 짧은 시간내에 풀에 반납되므로 임의로 반납시켜 주지 않음.
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

        if (dist < player_find_range) // 발견 범위
        {
            nav_mesh_agent.isStopped = false;

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true ||
               unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") == true)
            {
                nav_mesh_agent.isStopped = true;
                //unit_animation.SetBool("IsRun", false);
                return;
            }

            if (dist <= nav_mesh_agent.stoppingDistance) // 공격 범위
            {
                if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Run") == true)
                    unit_animation.SetBool("IsRun", false);

                if (is_doing_attack_motion_cor == null)
                    is_doing_attack_motion_cor = StartCoroutine(OnAttackMotion());
            }

            else
            {
                if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true ||
                    unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") == true)
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

        else // 발견 범위 밖
        {
            unit_animation.SetBool("IsRun", false);
            unit_animation.SetBool("IsAttack", false);

            nav_mesh_agent.isStopped = true;
        }
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

        unit_animation.SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.2f);

        is_doing_attack_motion_cor = null;
    }

    private void OffAttackMotion()
    {
        unit_animation.SetBool("IsAttack", false);
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
}
