using Core.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Shield
{
    [CreateAssetMenu(fileName = "Data", menuName = "Shield/Create Shield")]
    public class ShieldBase: ScriptableObject
    {

        public float damageReduction;
        public bool reflection;
        public float slowDownRate;
       
        
    }


   
}
