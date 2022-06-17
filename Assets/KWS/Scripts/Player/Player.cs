using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit
{
    private bool is_damaged = false;
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
        {
            is_damaged = false;
            return;
        }

        is_damaged = true;
        base.DamagedAnimation();
    }

    public override void ReceiveDamage(float damage, Vector3 hit_pos)
    {
        if (unit_animation.GetCurrentAnimatorStateInfo(0).IsName("Damaged") == true) return;
        if (is_damaged == true) return;

        base.ReceiveDamage(damage, hit_pos);
        
        if (IsDead() == false) DamagedAnimation();
    }

    public override IEnumerator ShowDamageText(float damage)
    {
        GameObject obj = ObjectPoolingManager.Instance.GetObjectFromPoolingQueue("DamageCanvasPlayer");
        Text text = obj.transform.GetChild(0).GetComponent<Text>();

        int i_damage = (int)damage;

        text.text = i_damage.ToString();
        obj.transform.SetParent(transform);

        ObjectPoolingManager.Instance.InitDamageTextTransform(ref obj);

        yield return new WaitForSeconds(1f);

        ObjectPoolingManager.Instance.ReturnObjectToPoolingQueue("DamageCanvasPlayer", obj);
    }

    void EndGetDamageMotion()
    {
        Debug.Log("��");
        is_damaged = false;
    }
}
