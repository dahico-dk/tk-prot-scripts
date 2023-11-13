using Core.Movement.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Combat.Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        public Transform spawnpoint;
        public float spawnRate = 3f;
        public int maxMelee = 20;
        public int maxRanged = 10;
        public bool spawn = false;
        UnityEngine.Object[] meleenemies;
        UnityEngine.Object[] rangedenemies;
        public GameObject target;

        private PoolManager poolmanager;
        private void Awake()
        {
            meleenemies = Resources.LoadAll("Enemies/Melee", typeof(GameObject));
            rangedenemies = Resources.LoadAll("Enemies/Ranged", typeof(GameObject));
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();

        }

        void Start()
        {
            StartCoroutine("Spawn");
        }
        // Maybe Mark enemies for spawn point
        void FixedUpdate()
        {

        }
        IEnumerator Spawn()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            var meleecount = enemies.Where(a => a.GetComponent<EnemyCombat>().equippedweapon.melee).Count();
            var rangedcount = enemies.Count() - meleecount;
            var maxEnemy = maxMelee + maxRanged;
            while (true)
            {
                float nextenemy = rangedcount < maxRanged ? Mathf.Ceil(UnityEngine.Random.Range(0, 1)) : 0;
                if (enemies.Length <= maxEnemy)
                {
                    var newenemy = poolmanager.GetPooledEnemy(nextenemy == 0);
                    if (newenemy != null)
                    {
                        newenemy.transform.position = new Vector3(spawnpoint.position.x, 0, spawnpoint.position.z);
                        newenemy.SetActive(true);
                       
                        StartCoroutine(RadiusChecker(newenemy));
                        yield return new WaitForSeconds(spawnRate);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                }
            }


        }

        IEnumerator RadiusChecker(GameObject instantiated)
        {
            yield return new WaitUntil(() => Vector3.Distance(transform.position, instantiated.transform.position) > 10);
            var agent = instantiated.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.radius = 2f;
            }
            yield return null;
        }
        private GameObject RandomEnemy(float meleeorranged)
        {
            //0 melee 1 ranged                     
            var returner = meleeorranged == 0 ? meleenemies[UnityEngine.Random.Range(0, meleenemies.Length)] : rangedenemies[UnityEngine.Random.Range(0, rangedenemies.Length)];
            return (GameObject)returner;
        }
    }
}
