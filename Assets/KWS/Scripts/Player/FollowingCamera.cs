using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target_transform; // 본인 캐릭터

    [SerializeField]
    private Transform anchor_transform; // 카메라의 기준점

    [SerializeField]
    private UIActiveGetKeyInput ui_active_get_key_input;

    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;

    private bool is_using_skill_slot = false;
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
        anchor_transform.position = target_transform.position;
    }
    void RotateCamera()
    {
        if (is_using_skill_slot == true) return;
        if (ui_active_get_key_input.IsActivedUI() == true) return;

        Vector2 mouse_input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 cam_angle = anchor_transform.rotation.eulerAngles;

        float x = cam_angle.x - mouse_input.y;

        x = (x < 180 ? Mathf.Clamp(x, -1f, 50f) : Mathf.Clamp(x, 335f, 361f));

        anchor_transform.rotation = Quaternion.Euler(x, cam_angle.y + mouse_input.x, cam_angle.z);
    }
}
