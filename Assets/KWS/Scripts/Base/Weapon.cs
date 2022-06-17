using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWSWeapon
{
    public class Weapon : MonoBehaviour
    {
        #region ¸â¹ö º¯¼ö
        private enum WeaponType { Melee, Range };

        [SerializeField]
        private WeaponType weapon_type;

        [SerializeField]
        protected BoxCollider melee_area;

        [SerializeField]
        protected TrailRenderer trail_effect;

        public TrailRenderer TrailEffect
        {
            set { trail_effect = value; }
        }
        
        [SerializeField]
        protected UnitStats unit_stats;

        [SerializeField]
        protected Animator unit_animation;
        #endregion

        public void ActiveOnWeaponArea()
        {
            if (weapon_type == WeaponType.Melee)
            {
                SwingOn();
            }
        }
        public void ActiveOffWeaponArea()
        {
            if (weapon_type == WeaponType.Melee)
            {
                SwingOff();
            }
        }
        private void SwingOn()
        {
            melee_area.enabled = true;
            trail_effect.enabled = true;
        }

        private void SwingOff()
        {
            melee_area.enabled = false;
            trail_effect.enabled = false;

            float tmp = trail_effect.time;
            
            trail_effect.time = 0;
            trail_effect.time = tmp;
        }
    }
}
