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
    private void Start()
    {
        base.Start();

        nav_mesh_agent = GetComponent<NavMeshAgent>();

        GetComponent<TriggerCallback>().collision_stay_event = FollowToPlayer;
        GetComponent<TriggerCallback>().collision_exit_event = (Collider) =>
        {
            nav_mesh_agent.enabled = false;
            unit_animation.SetBool("IsRun", false);
            unit_animation.SetBool("IsAttack", false);
        };
        
        GetComponent<TriggerCallback>().collision_stay_event = FollowToPlayer;
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
        return;

        if (collider.gameObject.CompareTag("Player") == true)
        {
            target_transform = collider.transform;
            nav_mesh_agent.enabled = true;

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false &&
                unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false)
                nav_mesh_agent.SetDestination(collider.transform.GetChild(0).position);

            float distance = Vector3.Distance(transform.position, collider.transform.GetChild(0).position);

            if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true)
                return;

            if (distance <= nav_mesh_agent.stoppingDistance)
            {
                //Debug.Log("도착");
                nav_mesh_agent.isStopped = true;
                nav_mesh_agent.updatePosition = false;
                nav_mesh_agent.updateRotation = false;

                unit_animation.SetBool("IsRun", false);
                unit_animation.SetBool("IsAttack", true);
            }

            else
            {
                // Debug.Log("비 도착 dist: " + distance);
                nav_mesh_agent.isStopped = false;
                nav_mesh_agent.updatePosition = true;
                nav_mesh_agent.updateRotation = true; 
                nav_mesh_agent.velocity = Vector3.zero;

                unit_animation.SetBool("IsAttack", false);
                unit_animation.SetBool("IsRun", true);
            }
        }
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
