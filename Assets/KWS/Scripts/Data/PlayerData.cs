using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public StatsTable stat_table;

    public int level;
    public int exp;
    public int gold;

    public Vector3 current_position;
    public Quaternion current_rotation;
}
