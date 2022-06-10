using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region ��� ����
    [SerializeField]
    private Transform follow_camera;

    [SerializeField]
    private float move_power;

    [SerializeField]
    private float rotate_power;

    [SerializeField]
    private float turn_power;

    [SerializeField]
    private PlayerAttack player_attack;
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
    //IEnumerator Move()
    void Move()
    {
        if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true)
                //yield return null; // ���� �� �ǰ� �ִϸ��̼� ���� ���� �ٱ� �Ұ�.
                return; // ���� �� �ǰ� �ִϸ��̼� ���� ���� �ٱ� �Ұ�.
        
        //move_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        bool is_move = move_input.magnitude != 0;

        if (is_move == false)
        {
            //player_attack.InitAttackAnimation();
            player_animator.SetBool("is_walk", is_move);

            Debug.Log("�� ���");

            if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == true &&
                player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f)
                ;//    player_animator.SetBool("is_walk", is_move);

            return;
        }

        player_animator.SetBool("is_walk", is_move);

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

        yield return new WaitForSeconds(0.35f);

        while (total_time < 1)
        {
            yield return null;
            total_time += Time.deltaTime * turn_power;
            transform.rotation = Quaternion.Slerp(transform.rotation, new_rotation, total_time);
        }

        yield return new WaitForSeconds(0.25f);
        is_turning = null;
    }
}