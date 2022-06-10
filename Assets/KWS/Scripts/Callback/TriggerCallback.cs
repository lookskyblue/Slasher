using System;
using UnityEngine;

public class TriggerCallback : MonoBehaviour
{
    public Action<Collider> collision_enter_event;
    public Action<Collider> collision_stay_event;
    public Action<Collider> collision_exit_event;

    private void OnTriggerEnter(Collider other) { if (collision_enter_event != null) collision_enter_event(other); }
    private void OnTriggerStay(Collider other) { if (collision_stay_event != null) collision_stay_event(other); }
    private void OnTriggerExit(Collider other) { if (collision_exit_event != null) collision_exit_event(other); }
}
