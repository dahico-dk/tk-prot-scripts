using Core.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Weapons
{
    [CreateAssetMenu(fileName = "Data", menuName = "Weapons/Create Special Weapon")]
    public class SpecialAttack : WeaponBase
    {
        public bool isItUltimate;     
        public float Cooldown;     
        //for ultimate purposes       
        public bool Active = false;      
        public GameObject[] UltimateSpawn;
             
    }


   
}
