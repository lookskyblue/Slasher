using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWSWeapon
{
    public class Weapon : MonoBehaviour
    {
        #region ¸â¹ö º¯¼ö
        private enum WeaponType { Melee, Range };

        [SerializeField] private WeaponType weapon_type;
        [SerializeField] protected BoxCollider[] melee_area;
        [SerializeField] protected TrailRenderer[] trail_effect;
        [SerializeField] protected UnitStats unit_stats;
        [SerializeField] protected Animator unit_animation;
        protected int now_idx = 0;
        #endregion

        public void ActiveOnWeaponArea(int idx)
        {
            if (weapon_type == WeaponType.Melee)
            {
                SwingOn(idx);
            }
        }
        public void ActiveOffWeaponArea(int idx)
        {
            if (weapon_type == WeaponType.Melee)
            {
                SwingOff(idx);
            }
        }
        private void SwingOn(int idx)
        {
            melee_area[idx].enabled = true;
            trail_effect[idx].enabled = true;

            now_idx = idx;
        }

        private void SwingOff(int idx)
        {
            melee_area[idx].enabled = false;
            trail_effect[idx].enabled = false;

            float tmp = trail_effect[idx].time;
            
            trail_effect[idx].time = 0;
            trail_effect[idx].time = tmp;
        }
    }
}
