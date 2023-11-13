using Core.Armor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Core.Movement;

namespace Core.Ranged
{
    class Frost : ElementBase
    {
          
        public override IEnumerator Element(GameObject target, float duration,float damage)
        {
            //start damage   ızın alıp ge        
            float firstspeed = 0;       
            //start slow
            if (target.tag == "Player")
            {
                var scr = target.GetComponent<PlayerMovement>();
                firstspeed = scr.speed;
                scr.speed = scr.speed / 2;
                StartCoroutine(base.Element(target, duration, damage));
                yield return new WaitForSeconds(duration);
                scr.speed = firstspeed;
            }
            else
            {
                //print("speed reduce for "+duration);
                //enemy               
                var agent = target.GetComponent<NavMeshAgent>();
                firstspeed = agent.speed;
                agent.speed = agent.speed / 2;
                StartCoroutine(base.Element(target, duration, damage));
                yield return new WaitForSeconds(duration);
                agent.speed = firstspeed;              
               // print("emded");
            }

            ;
        }

       

        

    }
}
