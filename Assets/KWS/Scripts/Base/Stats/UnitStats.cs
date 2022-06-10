using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Object/Stats")]

public class UnitStats : ScriptableObject
{
    [SerializeField]
    private int maxiam_hp = 5000;
    [SerializeField]
    private int maxiam_str = 1000;
    [SerializeField]
    private int maxiam_def = 1000;

    [SerializeField]
    private Action<int> accept_changed_hp;

    [SerializeField]
    private int default_hp = 200;

    [SerializeField]
    private int default_str = 20;

    [SerializeField]
    private int default_def = 20;

    [SerializeField]
    private int total_hp;

    [SerializeField]
    private int total_str;

    [SerializeField]
    private int total_def;

    public int Default_Hp { get { return default_hp; } }
    public int Default_Str { get { return default_str; } }
    public int Default_Def { get { return default_def; } }

    public int Total_Hp 
    {
        get { return total_hp; }
        set 
        {
            total_hp = value;
            total_hp = Mathf.Clamp(total_hp, 0, maxiam_hp);
        }
    }
    public int Total_Str 
    {
        get { return total_str; }
        set 
        {
            total_str = value;
            total_str = Mathf.Clamp(total_str, 0, maxiam_str);
        }
    }

    public int Total_Def
    { 
        get { return total_def; }
        set 
        {
            total_def = value;
            total_def = Mathf.Clamp(total_def, 0, maxiam_def);
        }
    }
    public Action<int> AcceptUsedPotion
    {
        get { return accept_changed_hp; }
        set 
        {
            accept_changed_hp = value;
        }
    }

    public void SetValidHpRange(ref float hp)
    {
        hp = Mathf.Clamp(hp, 0, total_hp);
    }
}
