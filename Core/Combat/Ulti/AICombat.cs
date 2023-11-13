
using Core.Armor;
using Core.Movement.AI;
using Core.Ranged;
using Core.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Combat.AI
{
    class AICombat : MonoBehaviour
    {
        public WeaponBase equippedweapon;
        public ArmorBase equippedarmor;
        private PoolManager poolmanager;
        private Animator anim;
        float timeSinceLastAttacks = 0;
       [HideInInspector] public GameObject target = null;      
        private void KingIsDead()
        {
            var movi = GetComponent<AIMovement>();
            movi.target = null;
        }
        private void Start()
        {
            anim = GetComponent<Animator>();
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
        }
        private void Update()
        {
            if (target != null)
            {
                Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
            }
        }
        Transform FindSpawnPoint()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "SpawnPoint")
                {
                    return child.transform;
                }
            }

            return null;
        }
        public void AttackTarget()
        {
            var health = target.GetComponent<Health>();
            var Ap = equippedweapon.attackPower;
            //print("Hit");
            health.TakeDamage(Ap, gameObject);
            if (!equippedweapon.melee)            
            {
                var wep = (Core.Weapons.Ranged)equippedweapon;
                //Vector3 relativePos = target.transform.position - transform.position;
                //transform.rotation = Quaternion.LookRotation(relativePos);
                transform.LookAt(target.transform);
                var spawnpoint = FindSpawnPoint();
              
                var typer = wep.rangedprefab.GetComponent<EnumHolder>().projType;
                GameObject projectile = poolmanager.GetPooledProjectile(typer);
                projectile.transform.position = spawnpoint.position;
                projectile.transform.rotation = spawnpoint.rotation;
                // GameObject projectile = Instantiate(wep.rangedprefab, spawnpoint.position, spawnpoint.rotation);

                GetComponentInChildren<ParticleSystem>().Play();
                var proj = projectile.GetComponent<Projectile>();
                proj.parent = this.gameObject;
                proj.weapontype = (Core.Weapons.Ranged)equippedweapon;
            }
        }
        internal void TriggerAttack(GameObject _target)
        {
            target = _target;
            timeSinceLastAttacks += Time.deltaTime;
            if (timeSinceLastAttacks > equippedweapon.attackSpeed)
            {
                timeSinceLastAttacks = 0;
                anim.SetTrigger("Attack");
            }
        }
    }
}
