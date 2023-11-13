using Core.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Weapons
{
    [CreateAssetMenu(fileName = "Data", menuName = "Weapons/Create Melee Weapon")]
    class Melee: WeaponBase
    {
        
        public float damageMultiplier = 1;
        public ProjectileType typer;

    }
}
