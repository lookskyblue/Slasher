using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidInfo : MonoBehaviour
{
    [SerializeField] private string raid_name;
    [SerializeField] private int entry_level;
    [SerializeField] private int compensation_gold;
    [SerializeField] private int compensation_exp;
    public string Raid_Name { get { return raid_name; } }
    public int Entry_Level { get { return entry_level; } }
    public int Compensation_Gold { get { return compensation_gold; } }
    public int Compensation_Exp { get { return compensation_exp; } }
}
