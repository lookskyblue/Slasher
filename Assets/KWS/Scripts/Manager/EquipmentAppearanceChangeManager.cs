using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentAppearanceChangeManager : MonoBehaviour
{
    [SerializeField]
    UnitStats player_stats;

    [SerializeField]
    private InteractionSlotEvent interaction_slot_event;

    [SerializeField]
    private PlayerWeapon player_weapon;

    #region 무기 멤버 변수
    private Dictionary<string, Mesh> sword_mesh_dic = new Dictionary<string, Mesh>();
    [SerializeField]
    private Mesh[] sword_meshs;
    [SerializeField]
    private MeshFilter my_sword_mesh;
    [SerializeField]
    private GameObject my_sword_obj_group;
    #endregion

    #region 방패 멤버 변수
    private Dictionary<string, Mesh> shield_mesh_dic = new Dictionary<string, Mesh>();
    [SerializeField]
    private GameObject my_shield_obj_group;
    [SerializeField]
    private MeshFilter my_shield_mesh;
    [SerializeField]
    private Mesh[] shield_meshs;
    #endregion

    private void Start()
    {
        interaction_slot_event.Mount_Sword = MountSword;
        interaction_slot_event.Mount_Shield = MountShield;
        interaction_slot_event.Unmount_Sword = UnmountSword;
        interaction_slot_event.Unmount_Shield = UnmountShield;

        InitSwordMeshDictionary();
        InitShieldMeshDictionary();
    }

    void InitSwordMeshDictionary()
    {
        for (int i = 0; i < sword_meshs.Length; i++)
        {
            sword_mesh_dic.Add(sword_meshs[i].name, sword_meshs[i]);
        }
    }

    void InitShieldMeshDictionary()
    {
        for (int i = 0; i < shield_meshs.Length; i++)
        {
            shield_mesh_dic.Add(shield_meshs[i].name, shield_meshs[i]);
        }
    }
    private void MountSword(string item_name, int str)
    {
        if (sword_mesh_dic.ContainsKey(item_name) == false)
        {
            Debug.Log("무기 딕셔너리 오류");
            return;
        }

        //1. 무기 그룹 오브젝트 킴
        my_sword_obj_group.SetActive(true);
        my_sword_mesh.mesh = sword_mesh_dic[item_name];

        //5. 스텟 변경
        player_stats.Total_Str += str;
    }
    private void MountShield(string item_name, int def)
    {
        if (shield_mesh_dic.ContainsKey(item_name) == false)
        {
            Debug.Log("방패 딕셔너리 오류");
            return;
        }

        //1. 방패 그룹 오브젝트 킴
        my_shield_obj_group.SetActive(true);
        my_shield_mesh.mesh = shield_mesh_dic[item_name];

        //5. 스텟 변경
        player_stats.Total_Def += def;
    }
    private void UnmountSword(int str)
    {
        player_stats.Total_Str -= str;
        my_sword_obj_group.SetActive(false);
    }
    private void UnmountShield(int def)
    {
        player_stats.Total_Def -= def;
        my_shield_obj_group.SetActive(false);
    }
}
