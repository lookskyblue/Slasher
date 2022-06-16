using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    private void Start()
    {
        base.Start();
    }

    public override void DamagedAnimation()
    {
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack0") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack1") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("attack2") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Skill1") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("GroundCrack") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Smash") == true ||
           unit_animation.GetCurrentAnimatorStateInfo(0).IsName("CannonShot") == true)
            return;

        base.DamagedAnimation();
        //if(unit_animation.GetCurrentAnimatorStateInfo(0).IsName)
    }
}
