using Core.Combat.Enemy;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Movement.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Rigidbody rb;
        [SerializeField] private GameObject target;
        private Animator animator;

        public bool move = false;
        EnemyCombat combat;
        GameObject summonPortal;
        GameObject insantiatedsummonPortal;
        private bool searchAura = true;
        [HideInInspector] public bool summoned;
        float attackRange;
        private GameManager GameManager;
        private bool targetAssigned = false;
        void SetTarget()
        {
            if (GameManager.ultiState)
            {
                if (!targetAssigned)
                {
                    target = FindUltiSpawn();
                    targetAssigned = true;
                }
            }
            else
            {
                if (targetAssigned)
                {
                    targetAssigned = false;
                    target = null;
                }
                if (target == null)
                {
                    target = GameObject.FindGameObjectWithTag("Player");
                }
            }


            //if (agent!=null)
            //{
            //    agent.stoppingDistance=
            //}
        }
        private GameObject FindUltiSpawn()
        {
            var ultispawns = GameObject.FindGameObjectsWithTag("UISpawn");
            foreach (var item in ultispawns)
            {
                var uc=item.GetComponent<UltimateCompanion>();
                if (!uc.targeted)
                {
                    return item;
                }
            }
            //there is no non targeted ulti comp
            System.Random rnd = new System.Random();
            return ultispawns[rnd.Next(0,ultispawns.Count()-1)];

        }
        void Start()
        {
            var mng = GameObject.FindGameObjectWithTag("GameManager");
            GameManager = mng.GetComponent<GameManager>();
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            combat = GetComponent<EnemyCombat>();
            attackRange = combat.equippedweapon.attackradius;
            animator = GetComponent<Animator>();
            summonPortal = Resources.Load<GameObject>("Portal_Hero");
            if (summoned)
            {
                CreatePortal();
            }
            if (gameObject.tag == "Undead")
            {
                StartCoroutine("DisasterScenerio");
                agent.enabled = false;
                rb.isKinematic = true;
                move = false;
            }
            else
            {
                Finisher();
            }
        }
        public IEnumerator KnockBack(Vector3 force)
        {

            print("Knockback");
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(force, ForceMode.Impulse);
            print("waitforseconds");
            yield return new WaitForSeconds(1);
            print("returnfromseconds");
            print(rb.velocity.magnitude);
            yield return new WaitUntil(() => rb.velocity.magnitude == 0);
            print("stopped");
            rb.isKinematic = true;
            agent.enabled = true;
            rb.useGravity = false;


        }
        public IEnumerator KnockBack(Vector3 force, GameObject destroyer)
        {

            //print("Knockback");
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(force, ForceMode.Impulse);
            destroyer.GetComponent<MeshRenderer>().enabled = false;
            //print("waitforseconds");            
            yield return new WaitForSeconds(1);
            //print("returnfromseconds");
            // print(rb.velocity.magnitude);
            if (rb != null)
            {
                yield return new WaitUntil(() => target.transform.position.y < 1.1f);
                // print("stopped");
                rb.isKinematic = true;
                agent.enabled = true;
                rb.useGravity = false;

            }
            else
            {
                yield return null;
            }
            Destroy(destroyer);
        }
        void DisableAura()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Aura")
                {
                    child.GetComponent<ParticleSystem>().Stop();
                    searchAura = false;

                }
            }


        }
        void CreatePortal()
        {

            var newer = Instantiate(summonPortal, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            newer.transform.parent = transform;
        }
        void FixedUpdate()
        {
            try
            {
                if (target)
                {
                    var dist = Vector3.Distance(target.transform.position, transform.position);
                    if (dist >= attackRange)
                    {
                        if (move)
                        {
                            animator.SetFloat("Speed", agent.speed);
                            agent.isStopped = false;
                            agent.destination = target.transform.position;
                        }
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
            catch (System.Exception ex)
            {
                print(this.name);
                throw;
            }
        }
        private void Update()
        {
            SetTarget();
        }

        IEnumerator DisasterScenerio()
        {
            yield return new WaitForSeconds(3.5f);
            if (rb.isKinematic || !agent.enabled || !move)
            {
                Finisher();
                yield return null;
            }
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
        //disable animation effects
        void Finisher()
        {
            agent.enabled = true;
            move = true;
            rb.isKinematic = false;
            DisableAura();
        }
        void FinisherArcher()
        {
            Finisher();
        }
        void FinisherDef()
        {
            Finisher();
        }



    }
}
