using Core.Armor;
using Core.Combat.AI;
using Core.Combat.Enemy;
using Core.Weapons;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Movement.AI
{
    class AIMovement:MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent agent;
        private Rigidbody rb;
        public GameObject target;
        public bool corot;
        public bool move = false;
        AICombat combat;
        Animator animator;
        float attackRange;
        float maxRanged;
        GameObject summonPortal;
        private PoolManager poolmanager;
        public ParticleSystem Portal;
        //[SerializeField]WeaponBase ranged;
        //[SerializeField] WeaponBase melee;
        void Start()
        {
            //summonPortal = Resources.Load<GameObject>("Portal");
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            combat = GetComponent<AICombat>();
            animator = GetComponent<Animator>();
            attackRange = combat.equippedweapon.attackradius;
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
            //CreatePortal();           
        }
        private void OnEnable()
        {
            Portal.Play();
        }
        private void OnDisable()
        {
            //Portal.Play();
        }
        void CanMove()
        {
            print("Activated");
            move = true;
        }      
        void Update()
        {                
            if (move && target)
            {
                var dist = Vector3.Distance(target.transform.position, transform.position);
                if (dist >= attackRange)
                {
                    animator.SetFloat("Speed", agent.speed);
                    agent.isStopped = false;
                    agent.destination = target.transform.position;
                }
                else
                {
                    if (agent.isActiveAndEnabled)
                    {
                        agent.isStopped = true;
                    }
                    animator.SetFloat("Speed", 0);
                    combat.TriggerAttack(target);
                }
            }
            else
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = true;
                }
            }
        }
        public bool HasTarget()
        {
            if (target&&target.GetComponent<Health>().CheckAlive())
            {
                return true;
            }
            return false;
        }
        //void CreatePortal()
        //{
        //    var newer = poolmanager.GetPortal();
        //    newer.transform.parent = transform;
        //    newer.SetActive(true);
        //    newer.GetComponent<ParticleSystem>().Play();         
        //}
        void DisableAura()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Aura")
                {
                    child.GetComponent<ParticleSystem>().Stop();                   
                }
            }
        }

       

        //void Finisher()
        //{
        //    agent.enabled = true;
        //    move = true;
        //    rb.isKinematic = false;
        //    DisableAura();
        //}

        //private GameObject FindTarget()
        //{
        //    if (target.gameObject==null)
        //    {
        //        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //        var mn = 0;
        //        float distance=Vector3.Distance(transform.position, enemies.FirstOrDefault().transform.position);
        //        GameObject chosen= enemies.FirstOrDefault();
        //        foreach (var item in enemies)
        //        {
        //            var comb = item.GetComponent<EnemyCombat>();


        //            var newdist = Vector3.Distance(transform.position, item.transform.position);
        //            if (newdist < distance&&!comb.targeted)
        //            {
        //                distance = newdist;
        //                chosen = item;
        //            }                   
        //        }

        //        chosen.GetComponent<EnemyCombat>().targeted = true;

        //        return chosen;
        //        //Destroy(chosen.gameObject);
        //    }
        //    else
        //    {
        //        return target;
        //    }
        //}
    }
}
