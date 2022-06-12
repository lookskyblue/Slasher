using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDragAndDropContainer : MonoBehaviour
{
    //public SkillInfo my_skill_info;

    [SerializeField]
    private Image image;
    private SkillInfo? skill_info = null;

    public Image Image
    {
        get { return image; }
        set { image = value; }
    }

    public SkillInfo? Skill_Info
    {
        get { return skill_info; }
        set { skill_info = value; }
    }

    private void Start()
    {
        skill_info = null;
        image.gameObject.SetActive(false);
    }
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Alpha1) == true)
    //    {
    //        Debug.Log("스킬정보에 값 대입.");

    //        Skill_Info = my_skill_info;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Alpha2) == true)
    //    {
    //        Debug.Log("스킬정보에 널 대입.");

    //        Skill_Info = null;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Alpha3) == true)
    //    {
    //        Debug.Log("스킬정보 출력");

    //        if(Skill_Info == null)
    //        {
    //            Debug.Log("널이라 리턴함");
    //            return;
    //        }

    //        Debug.Log("name: " + Skill_Info.Value.name);
    //        Debug.Log("level: " + Skill_Info.Value.possible_level);
    //        Debug.Log("taken_point: " + Skill_Info.Value.taken_point);
    //        Debug.Log("limit_point: " + Skill_Info.Value.limit_point);
    //        Debug.Log("mp_cost: " + Skill_Info.Value.mp_cost);
    //        Debug.Log("cool_time: " + Skill_Info.Value.cool_time);
    //        Debug.Log("animation_key: " + Skill_Info.Value.animation_key);
    //        Debug.Log("particle_key: " + Skill_Info.Value.particle_key);
    //        Debug.Log("particle_key: " + Skill_Info.Value.particle_key);
    //    }
    //}
}
