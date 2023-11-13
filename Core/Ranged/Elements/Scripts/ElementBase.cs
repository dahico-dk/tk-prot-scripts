using Assets.Scripts.Core;
using Assets.Scripts.Core.Combat.Ranged;
using Core.Armor;
using Core.Combat.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Core.Weapons;
using Core.Combat;

namespace Core.Ranged
{
    public class ElementBase : MonoBehaviour, IElement
    {
        public GameObject effect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Health health;
        public WeaponBase equippedweapon;
        public virtual void ApplyElement(GameObject target, float duration, float damage=0)
        {
            //don't stack
            health = target.GetComponent<Health>();
            if (!health.elemented&&ElementChance(target))
            {
                health.elemented = true;              
                StartCoroutine(Element(target, duration,damage));
            }
        }       
        public virtual IEnumerator ApplyEffectToEnemy()
        {
            yield return null;
        }
        public virtual IEnumerator Element(GameObject target, float duration, float damage)
        {
            var eleffect = Instantiate(equippedweapon.elementedEffect, target.transform);
            eleffect.transform.localPosition = new Vector3(0, target.transform.lossyScale.y / 2, 0);          
            float totalTime = 0;
            while (totalTime < duration)
            {
                print("applying" + totalTime+ " damage "+damage+ "duration " +duration);
                var hlth = target.GetComponent<Health>();
                if (health!=null)
                {                    
                    hlth.TakeDamage(damage);
                    totalTime++;
                    yield return new WaitForSeconds(1);
                }             
            }
            health.elemented = false;
            print("damage ended");
            Destroy(eleffect.gameObject);
            yield return null;
            //Destroy(this);
        }
        public bool ElementChance(GameObject target)
        {
            try
            {
                var dice = UnityEngine.Random.value * 100;
                print(dice);
                ArmorBase armor;
                if (target.gameObject.tag == "Enemy")
                {
                    var cmb =target.GetComponent<EnemyCombat>();
                    armor = cmb.equippedarmor;
                }
                else
                {
                    var cmb = target.GetComponent<PlayerCombat>();
                    armor = cmb.equippedarmor;
                }   
                //velevki
                float resistance = armor != null ? armor.elementalresistance : 0;
                var finaljudgment = resistance != 0 ? dice - ((dice / resistance) * 100) : dice;
                if (finaljudgment > 51)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                print("Exception occured=>"+ex.Message+" Target is=>"+target.name );
                
                throw;
            }
        }

        

       
    }
}
