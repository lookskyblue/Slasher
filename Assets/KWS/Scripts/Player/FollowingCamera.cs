using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private Transform target_transform; // 본인 캐릭터
    [SerializeField] private Transform anchor_transform; // 카메라의 기준점
    [SerializeField] private InteractionSlotEvent interaction_slot_event;
    [SerializeField] private UIActiveGetKeyInput ui_active_get_key_input;
    private Camera camera;
    private bool is_using_skill_slot = false;
    private bool is_shaking_camera = false;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }
    private void Start()
    {
        interaction_slot_event.UsingSkillSlot += UsingSkillSlot;
    }

    private void UsingSkillSlot(bool value)
    {
        is_using_skill_slot = value;
    }
    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
        RotateCamera();
        FollowingCameraToCharacter();
    }

    void FollowingCameraToCharacter()
    {
        if (is_shaking_camera == true) return;

        anchor_transform.position = target_transform.position;
    }
    void RotateCamera()
    {
        if (is_using_skill_slot == true) return;
        if (ui_active_get_key_input.IsActivedUI() == true) return;
        if (GameManager.instance.IsDoingOtherTask() == true) return;

        Vector2 mouse_input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 cam_angle = anchor_transform.rotation.eulerAngles;

        float x = cam_angle.x - mouse_input.y;

        x = (x < 180 ? Mathf.Clamp(x, -1f, 50f) : Mathf.Clamp(x, 335f, 361f));

        anchor_transform.rotation = Quaternion.Euler(x, cam_angle.y + mouse_input.x, cam_angle.z);
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCor());
    }

    IEnumerator ShakeCameraCor()
    {
        is_shaking_camera = true;

        float total_time = 0f;
        Vector3 origin_pos = anchor_transform.position;

        while (total_time < 0.15f)
        {
            total_time += Time.deltaTime;
            Vector3 new_pos = Random.insideUnitSphere * 0.1f + origin_pos;
            anchor_transform.position = new_pos;

            yield return null;
        }

        is_shaking_camera = false;
    }
    public void ZoomInAndOut()
    {
        StartCoroutine(ZoomInAndOutCor());
    }
    IEnumerator ZoomInAndOutCor()
    {
        yield return StartCoroutine(ZoomIn());
        yield return new WaitForSeconds(1.1f);
        yield return StartCoroutine(ZoomOut());
    }
    IEnumerator ZoomIn()
    {
        for (int i = 0; i < 10; i++)
        {
            camera.fieldOfView -= 1;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ZoomOut()
    {
        for (int i = 0; i < 10; i++)
        {
            camera.fieldOfView += 1;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
