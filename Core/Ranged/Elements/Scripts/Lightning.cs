using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Ranged
{
    public class Lightning : ElementBase
    {
        public override IEnumerator Element(GameObject target, float duration, float damage)
        {
            float totalTime = 0;
            float range = 4;
            var health = target.GetComponent<Health>();
            var layerMask = LayerMask.GetMask("Enemy");
            while (totalTime < duration)
            {                              
                if (health != null)
                {
                    health.TakeDamage(damage);
                    var enemies = Physics.OverlapSphere(target.transform.position, range, layerMask);
                    //apply jump animation
                    foreach (var item in enemies)
                    {
                        if (item!=target)
                        {
                            item.GetComponent<Health>().TakeDamage(damage / 100 * 30);
                            yield return new WaitForSeconds(0.1f);
                        }                      
                    }
                    //deltatimeda olur aslında
                    totalTime++;
                    yield return new WaitForSeconds(1);
                }
                yield return null;
            }
            health.elemented = false;
            print("damage ended");
            Destroy(this);
        }
    }
}
