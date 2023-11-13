using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Core.Weapons;
using Core.Ranged;

namespace Core.Weapons
{
    [CreateAssetMenu(fileName = "Data", menuName = "Weapons/Create Ranged Weapon")]
    public class Ranged:WeaponBase
    {
        public bool aoe;      
        public float aoerange;
        public GameObject rangedprefab;
        public float projectilespeed;
        public AudioClip launchSound;
        public AudioClip explosionSound;
                   


    }
}
