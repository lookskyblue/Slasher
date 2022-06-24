using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Object/Stats")]

public class UnitStats : ScriptableObject
{
    public float TestExp;

    private Action on_level_up;
    public Action On_Level_Up
    {
        get { return on_level_up; }
        set { on_level_up = value; }
    }

    private Action on_str_change;
    public Action On_Str_Change
    {
        get { return on_str_change; }
        set { on_str_change = value; }
    }

    private Action on_def_change;
    public Action On_Def_Change
    {
        get { return on_def_change; }
        set { on_def_change = value; }
    }

    [SerializeField] private int level;
    [SerializeField] private float max_exp = 200;
    [SerializeField] private float exp = 0;
    [SerializeField] private int maxiam_hp = 5000;
    [SerializeField] private int maxiam_mp = 3000;
    [SerializeField] private int maxiam_str = 1000;
    [SerializeField] private int maxiam_def = 1000;

    private Action<int, int> accept_changed_hp;
    private Action<int> on_change_mp;

    [SerializeField] private int default_hp = 200;
    [SerializeField] private int default_mp = 150;
    [SerializeField] private int default_str = 20;
    [SerializeField] private int default_def = 20;
    [SerializeField] private int total_hp;
    [SerializeField] private int total_mp;
    [SerializeField] private int total_str;
    [SerializeField] private int total_def;
    [SerializeField] private int total_gold;

    public int Default_Hp 
    { 
        get { return default_hp; }
        set { default_hp = value; }
    }

    public int Default_Mp 
    {
        get { return default_mp; } 
        set { default_mp = value; }
    }

    public int Default_Str 
    {
        get { return default_str; } 
        set { default_str = value; }
    }
    public int Default_Def 
    {
        get { return default_def; } 
        set { default_def = value; } 
    }
    public int Total_Hp 
    {
        get { return total_hp; }
        set 
        {
            total_hp = value;
            total_hp = Mathf.Clamp(total_hp, 0, maxiam_hp);
        }
    }

    public int Total_Gold
    {
        get { return total_gold; }
        set { total_gold = value; }
    }

    public float Total_Exp
    {
        get { return exp; }
        set { exp = value; }
    }
    public int Total_Mp
    {
        get { return total_mp; }
        set
        {
            total_mp = value;
            total_mp = Mathf.Clamp(total_mp, 0, maxiam_mp);
        }
    }
    public int Total_Str 
    {
        get { return total_str; }
        set 
        {
            total_str = value;
            total_str = Mathf.Clamp(total_str, 0, maxiam_str);
            on_str_change?.Invoke();
        }
    }

    public int Total_Def
    { 
        get { return total_def; }
        set 
        {
            total_def = value;
            total_def = Mathf.Clamp(total_def, 0, maxiam_def);
            on_def_change?.Invoke();
        }
    }
    public Action<int, int> AcceptUsedPotion
    {
        get { return accept_changed_hp; }
        set 
        {
            accept_changed_hp = value;
        }
    }

    public Action<int> On_Change_Mp
    {
        get { return on_change_mp; }
        set 
        {
            on_change_mp = value;
        }
    }

    public int GetLevel { get { return level; } }
    public int LevelUp 
    {
        set 
        {
            if (30 <= level) return;

            level = value;
            On_Level_Up.Invoke();
        } 
    }

    public float MaxExp
    {
        get { return max_exp; }
        set { max_exp = value; }
    }

    public void SetValidHpRange(ref float hp)
    {
        hp = Mathf.Clamp(hp, 0, total_hp);
    }
}
