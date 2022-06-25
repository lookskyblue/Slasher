using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidMapTriggerCallback : TriggerCallback
{
    void Start()
    {
        GameManager.instance.GetRaidBoardManager.GetTriggerCallback(ref collision_enter_event, ref collision_exit_event);
    }
}
