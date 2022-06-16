using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Unit
{
    private NavMeshAgent nav_mesh_agent;
    private Action<bool> death_callback;
    private bool is_main_boss;
    private Transform target_transform;
    [SerializeField]
    private float attack_delay;
    private Coroutine is_doing_attack_motion_cor = null;
    private Coroutine is_doing_follow_cor = null;
    private void Start()
    {
        base.Start();

        nav_mesh_agent = GetComponent<NavMeshAgent>();

        GetComponent<TriggerCallback>().collision_enter_event = FollowToPlayer;
        GetComponent<TriggerCallback>().collision_exit_event = (Collider) =>
        {
            Debug.Log("퇴장");

            if (is_doing_follow_cor != null)
            {
                StopCoroutine(is_doing_follow_cor);
                is_doing_follow_cor = null;
            }

            nav_mesh_agent.enabled = false;
            unit_animation.SetBool("IsRun", false);
            unit_animation.SetBool("IsAttack", false);
        };
        
        //GetComponent<TriggerCallback>().collision_stay_event = FollowToPlayer;
        InstancingMaterial();
    }
    public void Init(Action<bool> death_callback, bool is_main_boss)
    {
        this.death_callback = death_callback;
        this.is_main_boss = is_main_boss;
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

    void ReportDeath()
    {
        death_callback(is_main_boss);
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(3f); // 다른 풀링 이펙트들은 3초 보다 훨씬 짧은 시간내에 풀에 반납되므로 임의로 반납시켜 주지 않음.
        Destroy(gameObject);
        //ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("Orc", this.gameObject);
    }
    void EffectReset()
    {
        renderer.material.color = origin_color;
    }
    void FollowToPlayer(Collider collider)
    {
        if (is_doing_follow_cor != null) return;
        if (collider.gameObject.CompareTag("Player") == false) return;

        is_doing_follow_cor = StartCoroutine(Follow(collider));
    }

    IEnumerator Follow(Collider collider)
    {
        Debug.Log("4");

        target_transform = collider.transform;
        nav_mesh_agent.enabled = true;

        while (true)
        {
            Debug.Log("5");

            float distance = Vector3.Distance(transform.position, collider.transform.GetChild(0).position);

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false)
            {
                if (distance <= nav_mesh_agent.stoppingDistance)
                {
                    nav_mesh_agent.isStopped = true;
                    nav_mesh_agent.updatePosition = false;
                    nav_mesh_agent.updateRotation = false;

                    if (is_doing_attack_motion_cor == null)
                    {
                        unit_animation.SetBool("IsRun", false);

                        is_doing_attack_motion_cor = StartCoroutine(OnAttackMotion());
                    }
                }

                else
                {
                    nav_mesh_agent.isStopped = false;
                    nav_mesh_agent.updatePosition = true;
                    nav_mesh_agent.updateRotation = true;
                    nav_mesh_agent.velocity = Vector3.zero;

                    if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false &&
                        unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false)
                    {
                        yield return nav_mesh_agent.SetDestination(collider.transform.GetChild(0).position);
                        unit_animation.SetBool("IsRun", true);
                    }
                }
            }

            yield return null;
        }

        is_doing_follow_cor = null;
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

        yield return null;

        while (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") == true)
        {
            yield return null;
        }

        is_doing_attack_motion_cor = null;
    }

    void OffAttackMotion()
    {
        Debug.Log("Off");

        unit_animation.SetBool("IsAttack", false);
    }

    private void LookAtPlayerEvent()
    {
        if (target_transform == null) return;
        StartCoroutine(LookAtPlayer());
    }

    IEnumerator LookAtPlayer()
    {
        float total_time = 0f;
        Vector3 dir = target_transform.position - transform.position;

        while(total_time < 1)
        {
            yield return null;
            total_time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), total_time);
        }
    }
}
