using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageReceive
{
    void ReceiveDamage(float damage, Vector3 hit_pos);
}
