using Core.Armor;
using Core.Movement.Enemy;
using Core.Ranged;
using Core.Weapons;
using System;
using System.Collections;
using UnityEngine;

namespace Core.Combat.Enemy
{
    class EnemyCombat : MonoBehaviour
    {
        EnemyMovement mover;
        public WeaponBase equippedweapon;
        public ArmorBase equippedarmor;
        Transform rangedspawnpoint;
        Animator anim;
        private Rigidbody rb;
        LayerMask mask;
        float timeSinceLastAttacks = 0;
        [HideInInspector] public GameObject target = null;
        public bool targeted;
        private float animationtime=0;
       private float firstspeed;
        private PoolManager poolmanager;
      
       
        private void Start()
        {
           
            anim = GetComponent<Animator>();
            rangedspawnpoint = FindSpawnPoint();
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<EnemyMovement>();
            SetAnimationTime();
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
        }
        private void SetAnimationTime()
        {
            foreach (var item in anim.runtimeAnimatorController.animationClips)
            {
                if (item.name == "Attack")
                {
                    animationtime = item.length;
                }
            }
            if (animationtime <= 0)
            {
                animationtime = equippedweapon.attackSpeed;
            }
        }

        private void Update()
        {
            if (target != null)
            {
                Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
            }
        }
        public void TriggerAttack(GameObject _target)
        {        
            target = _target;
            StartCoroutine("MovementBehav");
            timeSinceLastAttacks += Time.deltaTime;
            if (timeSinceLastAttacks > equippedweapon.attackSpeed)
            {
                timeSinceLastAttacks = 0;
                anim.SetTrigger("Attack");
            }
        }
        IEnumerator MovementBehav()
        {
            mover.move = false;
            yield return new WaitForSecondsRealtime(animationtime);
            mover.move = true;
            yield return null;
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
            rb.isKinematic = true;
            //targeter = target;
            transform.LookAt(target.transform);            
            if (equippedweapon.melee)
            {
                if (Vector3.Distance(target.transform.position,transform.position)<equippedweapon.attackradius)
                {
                    var health = target.GetComponent<Health>();
                    mask = LayerMask.GetMask("Shield");
                    RaycastHit hit;
                    var Ap = equippedweapon.attackPower;
                    bool withShield = Physics.Raycast(transform.position, transform.forward * 1000, out hit, 100, mask);
                    if (withShield)
                    {
                        Ap = Ap - (Ap / 100 * target.GetComponent<PlayerCombat>().equippedshield?.damageReduction ?? 0);
                    }
                    //print("Hit");
                    health.TakeDamage(Ap);
                }
            }
            else
            {
                var wep = (Core.Weapons.Ranged)equippedweapon;
                //Vector3 relativePos = target.transform.position - transform.position;
                //transform.rotation = Quaternion.LookRotation(relativePos);
                transform.LookAt(target.transform);
                var typer = wep.rangedprefab.GetComponent<EnumHolder>().projType;
                GameObject projectile = poolmanager.GetPooledProjectile(typer);
                projectile.transform.position = rangedspawnpoint.position;
                projectile.transform.rotation = rangedspawnpoint.rotation;
                var proj = projectile.GetComponent<Projectile>();
                proj.parent = this.gameObject;
                proj.weapontype = (Core.Weapons.Ranged)equippedweapon;
            }
            rb.isKinematic = false;
        }
        public void AttackEnd()
        {
           mover.move = true;
        }
    }
}
