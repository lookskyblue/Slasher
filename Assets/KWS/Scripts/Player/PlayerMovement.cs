using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region 멤버 변수
    [SerializeField] private Player player;
    [SerializeField] private Transform follow_camera;
    [SerializeField] private float move_power;
    [SerializeField] private float rotate_power;
    [SerializeField] private float turn_power;
    [SerializeField] private PlayerAttack player_attack;
    [SerializeField] private AudioSource audio_source;
    [SerializeField] private AudioClip footstep_sound;
    private Animator player_animator;
    private Vector3 move_input;
    private Coroutine is_turning = null;
    #endregion
    private void Awake()
    {
        player_animator = GetComponent<Animator>();
    }
    private void Update()
    {
        GetMoveInput();
        Move();
    }

    void GetMoveInput()
    {
        move_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
    }

    private void FixedUpdate()
    {
        //StartCoroutine(Move());
        //Move();
    }
    void Move()
    {
        if (player.IsDead() == true) return;
        if (GameManager.instance.Is_Doing_Loading_Page == true) return;
        if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1") == true  ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("GroundCrack") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Smash") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("CannonShot") == true)
                //yield return null; // 공격 및 피격 애니메이션 중일 때는 뛰기 불가.
                return; // 공격 및 피격 애니메이션 중일 때는 뛰기 불가.

        //move_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);

        bool is_move = move_input.magnitude != 0;

        if (is_move == false)
        {
            player_animator.SetBool("is_walk", is_move);

            return;
        }

        player_animator.SetBool("is_walk", is_move);

        if (player_attack.Is_Doing_Attack_Stop_Cor == true) return;

        player_attack.StopAndAttackReset();

        if (is_move)
        {
            Vector3 look_forward = new Vector3(follow_camera.forward.x, 0f, follow_camera.forward.z).normalized;
            Vector3 move_dir = look_forward * move_input.y + follow_camera.right * move_input.x;
            Quaternion new_rotation = Quaternion.LookRotation(move_dir);

            transform.rotation = Quaternion.Slerp(transform.rotation, new_rotation, Time.deltaTime * rotate_power);
            //transform.position += move_dir * Time.deltaTime * move_power;
            //GetComponent<Rigidbody>().velocity = move_dir * move_power;
            GetComponent<Rigidbody>().AddForce(move_dir * move_power * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void CheckTurnTiming()
    {
        if (is_turning != null) return;

        Vector3 test_move_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);

        if (test_move_input.magnitude != 0)
        {
            Vector3 look_forward = new Vector3(follow_camera.forward.x, 0f, follow_camera.forward.z).normalized;
            Vector3 move_dir = look_forward * move_input.y + follow_camera.right * move_input.x;
            Quaternion new_rotation = Quaternion.LookRotation(move_dir);

            is_turning = StartCoroutine(TurnCor(new_rotation));
        }
    }

    IEnumerator TurnCor(Quaternion new_rotation)
    {
        float total_time = 0f;

        while (total_time < 1)
        {
            total_time += Time.deltaTime * turn_power;
            transform.rotation = Quaternion.Slerp(transform.rotation, new_rotation, total_time);

            yield return null;
        }

        is_turning = null;
    }
    void PlayStepSound()
    {
        audio_source.PlayOneShot(footstep_sound);
    }

    void StopSound()
    {
        // audio_source.PlayOneShot();
    }
}
