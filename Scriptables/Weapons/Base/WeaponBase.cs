using Core.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Weapons
{
    
    public class WeaponBase: ScriptableObject
    {
        public bool melee;
        public bool elemental;
        public float attackradius = 10;
        public float attackPower = 20;
        public bool knockBack;
        public float knockBackPower = 5f;
        public float attackSpeed;
        public GameObject element;
        public float elementalDuration;
        public float elementalAttackPower;
        public string EffectName;
        public GameObject elementedEffect;
        public GameObject hitEffect;




    }
}
