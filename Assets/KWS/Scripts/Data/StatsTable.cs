using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsTable
{
    public int str;
    public int def;
    public int hp;
    public int mp;
    public int hp_auto_recover_amount;
    public int mp_auto_recover_amount;
    public float max_exp;

    public StatsTable DeepCopy()
    {
        StatsTable new_stats_table = new StatsTable();

        new_stats_table.str = this.str;
        new_stats_table.def = this.def;
        new_stats_table.hp = this.hp;
        new_stats_table.mp = this.mp;
        new_stats_table.hp_auto_recover_amount = this.hp_auto_recover_amount;
        new_stats_table.mp_auto_recover_amount = this.mp_auto_recover_amount;
        new_stats_table.max_exp = this.max_exp;

        return new_stats_table;
    }
}
