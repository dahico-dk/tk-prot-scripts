using Core.Armor;
using Core.Ranged;
using Core.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Combat.Enemy
{
    public class EnemySpecial : MonoBehaviour
    {        
        public ArmorBase equippedarmor;
        NavMeshAgent agent;
        Transform rangedspawnpoint;
        Animator anim;              
        float timeSinceLastAttacks = 0;
        float timeSinceLastMovement = 100;
        [HideInInspector] public GameObject target = null;
       
        public float SummonCoolDown = 25f;
        public float WaitAfterSummon = 8f;
        public float WaitBetweenMovement = 8f;
       
        public GameObject DefaultZombie;
        public GameObject WarriorZombie;
        public GameObject ArcherZombie;
       // public GameObject NecroZombie;                        
        private Transform SummonPoint;
        bool rotating = false;
        private bool searchAura = true;
         

        private void Start()
        {
            anim = GetComponent<Animator>();
            rangedspawnpoint = FindSpawnPoint();
            target = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            SummonPoint = FindSpawnPoint();
            StartCoroutine("RandomWalk");
        }

        private void Update()
        {
            timeSinceLastMovement += Time.deltaTime;
            timeSinceLastAttacks += Time.deltaTime;
            anim.SetFloat("Speed", agent.velocity.magnitude);

        }
        private void FixedUpdate()
        {
            
        }

     
        public bool isNavMeshArrived()
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerator RandomWalk()
        {
            while (true)
            {
                print("comeback");
                UnityEngine.Random rnd = new UnityEngine.Random();
                var newloc = new Vector3(target.transform.position.x + UnityEngine.Random.Range(-25, 25), 0, target.transform.position.z + UnityEngine.Random.Range(-25, 25));
                agent.destination = newloc;
              
                yield return new WaitUntil(() => isNavMeshArrived());
                transform.LookAt(target.transform);
                timeSinceLastMovement = 0;
                if (timeSinceLastAttacks > SummonCoolDown)
                {
                    anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(WaitAfterSummon);
                }
                yield return new WaitForSeconds(WaitBetweenMovement);
            }
        }
        private void RotateToTarget()
        {
            Quaternion lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime);

        }
        private void Summon()
        {
           
            transform.LookAt(target.transform);
            var dice = UnityEngine.Random.Range(0, 100);
            GameObject objectToSpawn;
            //necro zombie simdilik iptal
            if (dice>65 &&dice<100)
            {
                objectToSpawn = ArcherZombie;
            }
            else if (dice > 35 && dice < 65)
            {
                objectToSpawn = WarriorZombie;
            }
            else
            {
                objectToSpawn = DefaultZombie;
            }
            var instade=Instantiate(objectToSpawn, SummonPoint.transform.position, Quaternion.identity);
            instade.GetComponent<Animator>().SetTrigger("Spawn");
            anim.SetFloat("Speed", -2f);
            timeSinceLastAttacks = 0;
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
            //targeter = target;
            timeSinceLastAttacks = 0;
          
        }
    }
}
