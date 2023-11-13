using Core.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Ranged
{
   public  interface IElement 
    {
       
        GameObject effect { get; set; }
        void ApplyElement(GameObject target,float duration,float damage);
     
        //void ApplyElement(GameObject target,float duration);

        IEnumerator Element(GameObject target, float duration, float damage);
        //IEnumerator Element(GameObject target, float duration);

        bool ElementChance(GameObject target);
    }
}
