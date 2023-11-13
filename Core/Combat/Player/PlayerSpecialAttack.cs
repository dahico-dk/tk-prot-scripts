using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Weapons;
using Core.Combat;
using Core.Movement.AI;
using System;
using System.Linq;
using Core.Combat.Enemy;
using Core.Animation;
using UnityEngine.UI;

namespace Core.Combat.Special
{


    public class PlayerSpecialAttack : MonoBehaviour
    {
        public SpecialAttack aoe;
        public SpecialAttack ultimate;
        float aoeCounter;
        float ultiCounter;
        bool Countable = true;
        PlayerCombat combat;
        PlayerAnimate animation;
        CombatMethods combatMethod;
        [SerializeField] GameObject AOEEffect;
        public Image aoeimage;
        public Image ultiimage;
        LayerMask mask;
        bool ultiState;
        private PoolManager poolmanager;
        private GameManager gmg;
        // Start is called before the first frame update       
        void Start()
        {
            aoeCounter = 999;
            ultiCounter = 999;
            mask = LayerMask.GetMask("Enemy");
            combat = GetComponentInParent<PlayerCombat>();
            combatMethod = GetComponent<CombatMethods>();
            animation = GetComponent<PlayerAnimate>();
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
            var mng = GameObject.FindGameObjectWithTag("GameManager");
            gmg = mng.GetComponent<GameManager>();
        }
        // Update is called once per frame
        void Update()
        {
            if (Countable)
            {
                aoeCounter += Time.deltaTime;
                ultiCounter += Time.deltaTime;
                if (aoeimage != null)
                {
                    if (aoeCounter > aoe.Cooldown)
                    {
                        aoe.Active = true;
                        aoeimage.color = Color.white;
                        //Show UI elements
                    }
                    else
                    {
                        aoeimage.color = Color.gray;
                        aoe.Active = false;
                    }
                }
                if (ultiCounter > ultimate.Cooldown)
                {
                    ultimate.Active = true;
                    ultiimage.color = Color.white;
                    //Show UI elements
                }
                else
                {
                    ultiimage.color = Color.gray;
                    ultimate.Active = false;
                }
                if (aoe.Active == true && Input.GetKeyDown(KeyCode.LeftControl))
                {
                    animation.Special(false);
                    aoeCounter = 0;
                }
                if (ultimate.Active == true && Input.GetKeyDown(KeyCode.Q) && !ultiState)
                {
                    animation.Special(true);
                    ultiCounter = 0;
                }
                if (ultiState)
                {
                    TargetAssigner();
                }

            }
        }
        void AOEBehaviour()
        {
            //animation.Special(false);
            var radius = Physics.OverlapSphere(transform.position, aoe.attackradius, mask);

            foreach (var enemy in radius)
            {
                combatMethod.TranslateDamage(enemy.gameObject, aoe.attackPower);
                combatMethod.Elemental(enemy.gameObject, aoe);
                combatMethod.KnockBack(enemy.gameObject, aoe, this.transform);
            }
            aoeCounter = 0;
            if (AOEEffect)
            {
                AOEEffect.GetComponent<ParticleSystem>().Play();
            }

        }
        void UltiBehaviour()
        {
            //animation.Special(true);
            //print("I must do some ulti la la la");

            //var thePosition = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * 6));
            //var thePosition2 = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * 4));
            //var thePosition3 = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * 2));
            //var thePosition4 = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * 0));
            //var thePosition5 = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * -2));
            //var thePosition6 = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * 4));

            var ultiheroes = poolmanager.GetHeroes();
            var startPos = ultimate.UltimateSpawn.Length;
            foreach (var item in ultiheroes)
            {
                var thePosition = transform.TransformPoint((-Vector3.forward * 6) + (Vector3.left * startPos));
                item.transform.position = thePosition;
                item.transform.rotation = transform.rotation;
                // var hero= Instantiate(item, thePosition, transform.rotation);
                item.SetActive(true);
                //var anim = item.GetComponent<Animator>();
                //anim.SetTrigger("Spawned");
                startPos -= 2;
            }
            gmg.ultiState = true;
            ultiState = true;
            ultiCounter = 0;
        }
        void TargetAssigner()
        {
            var uispawns = GameObject.FindGameObjectsWithTag("UISpawn");
            if (uispawns.Length > 0)
            {
                foreach (var item in uispawns)
                {
                    var movm = item.GetComponent<AIMovement>();
                    if (movm.HasTarget())
                    {
                        continue;
                    }
                    else
                    {
                        movm.target = FindNewTarget();
                    }
                }
            }
            else
            {
                gmg.ultiState = false;
                ultiState = false;
            }
        }
        private GameObject FindNewTarget()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            //closest enemy
            var unsorted = enemies.Where(a => a.GetComponent<Health>().CheckAlive()).OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position));
            var sorted = unsorted.Where(a => !a.GetComponent<EnemyCombat>().targeted).ToList();
            GameObject chosen;
            foreach (var item in sorted)
            {
                var comb = item.GetComponent<EnemyCombat>();
                if (comb.targeted)
                {
                    continue;
                }
                chosen = item;
                chosen.GetComponent<EnemyCombat>().targeted = true;
                return chosen;
            }
            //if we are here no untargeted element
            return unsorted.FirstOrDefault();
            //float distance = Vector3.Distance(transform.position, enemies.FirstOrDefault().transform.position);
            //GameObject chosen = enemies.Where(a=>!a.GetComponent<EnemyCombat>().targeted).FirstOrDefault();
            //foreach (var item in enemies)
            //{
            //    var comb = item.GetComponent<EnemyCombat>();
            //    if (comb.targeted)
            //    {
            //        continue;
            //    }
            //    var newdist = Vector3.Distance(transform.position, item.transform.position);
            //    if (newdist < distance && !comb.targeted)
            //    {
            //        distance = newdist;
            //        chosen = item;
            //    }
            //}

            //chosen.GetComponent<EnemyCombat>().targeted = true;

        }
    }
}
