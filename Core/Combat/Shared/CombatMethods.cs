using Core.Movement.Enemy;
using Core.Ranged;
using Core.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Combat
{
    public class CombatMethods:MonoBehaviour
    {
        LayerMask mask;
        private void Start()
        {
            mask = LayerMask.GetMask("Enemy");
        }
        public void AOEEffect(GameObject center, WeaponBase weapontype, GameObject caller)
        {
            var enemies = Physics.OverlapSphere(center.transform.position, weapontype.attackradius,mask);
            //v/*ar enemies = objects.Where(a => a.tag == "Enemy").ToArray();*/
            if (enemies.Length > 0)
            {
                foreach (var enemy in enemies)
                {                  
                    enemy.GetComponent<Health>()?.TakeDamage(weapontype.attackPower);
                    //try elemental
                    Elemental(enemy.gameObject,weapontype);
                    KnockBack(enemy.gameObject, weapontype,caller.transform);
                }

            }
        }
        public void KnockBack(GameObject target,WeaponBase weapontype,Transform caller,GameObject destroyer)
        {
            if (weapontype.knockBack && target.gameObject.tag != "Player")
            {
                Vector3 moveDirection = target.transform.position - caller.transform.position;
                //var direction = item.transform.forward.normalized;
                //item.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 500f);
                moveDirection.y = 1.5f;
                StartCoroutine(target.GetComponent<EnemyMovement>().KnockBack(moveDirection.normalized * weapontype.knockBackPower, destroyer));
            }
        }
        public void KnockBack(GameObject target, WeaponBase weapontype, Transform caller)
        {
            if (weapontype.knockBack && target.gameObject.tag != "Player")
            {
                Vector3 moveDirection = target.transform.position - caller.transform.position;
                //var direction = item.transform.forward.normalized;
                //item.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 500f);
                moveDirection.y = 1.5f;
                var componenet = target.GetComponent<EnemyMovement>();
                var ienum = componenet.KnockBack(moveDirection.normalized * weapontype.knockBackPower);
                StartCoroutine(ienum);
            }
        }  
        public void Elemental(GameObject target, WeaponBase weapontype)
        {
            //burası dıger attacklar ıcın daha generık olabılırdı.
            if (weapontype.elemental)
            {
                GameObject elementbase = Instantiate(weapontype.element);
                var element = elementbase.GetComponent<ElementBase>();
                element.equippedweapon = weapontype;
                element.ApplyElement(target.gameObject, weapontype.elementalDuration, weapontype.elementalAttackPower);
                Destroy(elementbase, (weapontype.elementalDuration * 2));
                
            }
        }
        public void TranslateDamage(GameObject target,float attackPower)
        {
            var health = target.GetComponent<Health>();
            health.TakeDamage(attackPower);
        }
    }
}
