
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Weapons;
using Assets.Scripts.Core;
using System.Linq;
using System;
using Core;
using Core.Movement.Enemy;
using Core.Combat;
using System.Diagnostics;

namespace Core.Ranged
{
    public class Projectile : MonoBehaviour
    {

        [HideInInspector] public GameObject parent;
        public Core.Weapons.Ranged weapontype;
        [HideInInspector] public float speed;
        CombatMethods combatMethods;
        [SerializeField] GameObject explosion;
        AudioSource source;       
        public GameObject caster;
        Rigidbody rbody;
        private bool jict = false;
        float jictimer = 0;
        bool collided = false;
        public bool shielded=false;
        private PoolManager poolmanager;
        private void Awake()
        {
            rbody = GetComponent<Rigidbody>();
            source = GetComponent<AudioSource>();
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
        }
        private void OnEnable()
        {
            jict = false;
            jictimer = 0;
            collided = false;
            speed = weapontype.projectilespeed;
            combatMethods = GetComponent<CombatMethods>();
            source.clip = weapontype.launchSound;
            //TODO: Uncomment
            //source.Play();
            if (explosion != null)
            {
                explosion.GetComponent<ParticleSystem>().Stop();
            }
        }
        private void Start()
        {
            speed = weapontype.projectilespeed;
            combatMethods = GetComponent<CombatMethods>();
            source.clip = weapontype.launchSound;          
        }
        // Update is called once per frame
        void Update()
        {
            jictimer += Time.deltaTime;
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed);
            if (jictimer > 5)
            {
                //just in case
                jict = true;
                this.gameObject.SetActive(false);
            }
        }
        //private void OnCollisionEnter(Collision target)
        //{
        //    if (!collided)
        //    {
        //        var healthComp = target.gameObject.GetComponent<Health>();
        //        if (target.gameObject.tag == "Enemy" && parent.tag != "Enemy")
        //        {
        //            print("1");
        //            collided = true;
        //            combatMethods.TranslateDamage(target.gameObject, weapontype.attackPower);
        //            if (weapontype.aoe)
        //            {
        //                combatMethods.AOEEffect(this.gameObject, weapontype, this.gameObject);
        //            }
        //            else if (weapontype.elemental)
        //            {
        //                combatMethods.Elemental(target.gameObject, weapontype);
        //                //combatMethods.KnockBack(target.gameObject, weapontype, transform, this.gameObject);
        //            }
        //        }
        //        else if (target.gameObject.tag == "Player" && parent.tag != "Player")
        //        {
        //            print("2");
        //            collided = true;
        //            var Ap = weapontype.attackPower;
        //            Health health;
        //            PlayerCombat combat;
        //            GameObject player = target.gameObject;
        //            if (target.gameObject.tag == "Shield")
        //            {
        //                health = target.gameObject.GetComponentInParent<Health>();
        //                combat = target.gameObject.GetComponentInParent<PlayerCombat>();
        //                //yandan ataklar için
        //                Ap = combat != null & combat.defending ? Ap - (Ap / 100 * target.gameObject.GetComponentInParent<PlayerCombat>().equippedshield?.damageReduction ?? 0) : Ap;
        //                player = target.transform.parent.gameObject;
        //            }
        //            else { health = target.gameObject.GetComponent<Health>(); }
        //            health.TakeDamage(Ap);
        //            //TODO: Bir ara düzelt. burda hata verme ve element uygulamama ihtimali var shield durumunda
        //            combatMethods.Elemental(player, weapontype);

        //        }
        //        else if (target.gameObject.tag == "Projectile" && target.transform.parent != parent)
        //        {
        //            print("3");
        //            collided = true;
        //            target.gameObject.SetActive(false);
        //        }
        //        else if (target.gameObject.tag == "Environment") { collided = true; }
        //        else
        //        {
        //            return;
        //        }
        //        StartCoroutine(Destroyer(target.gameObject));
        //    }
        //}
        IEnumerator Destroyer(GameObject target)
        {
            var type = weapontype.rangedprefab.GetComponent<EnumHolder>().projType;
            var inst = poolmanager.GetPooledExplosion(type);
            inst.transform.position = transform.position;
            if (inst != null)
            {
                //var inst = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);                                           
                if (inst.GetComponent<ProjectileExplosion>() == null)
                {
                    var comp = inst.AddComponent<ProjectileExplosion>();
                }
                inst.GetComponent<ProjectileExplosion>().StartCountDown();
            }
            source.clip = weapontype.explosionSound;
            source.Play();
            GetComponent<ParticleSystem>().Stop();
            GetComponent<Light>().enabled = false;
            if (target.GetComponent<Health>() != null ? target.GetComponent<Health>().elemented : false)
            {
                yield return new WaitUntil(() => !target.GetComponent<Health>().elemented);
            }
            this.gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider target)
        {
            Collide(target.gameObject);
        }

        public void Collide(GameObject target)
        {
            if (!collided)
            {
                var healthComp = target.gameObject.GetComponent<Health>();
                if (target.gameObject.tag == "Enemy" && parent.tag != "Enemy")
                {
                    print("1");
                    collided = true;
                    combatMethods.TranslateDamage(target.gameObject, weapontype.attackPower);
                    if (weapontype.aoe)
                    {
                        print("aoe");
                        combatMethods.AOEEffect(this.gameObject, weapontype, this.gameObject);
                    }
                    else if (weapontype.elemental)
                    {
                        print("elem");
                        combatMethods.Elemental(target.gameObject, weapontype);
                        //combatMethods.KnockBack(target.gameObject, weapontype, transform, this.gameObject);sssss
                    }
                }
                else if (target.gameObject.tag == "Player" && parent.tag != "Player")
                {
                    print("2");
                    collided = true;
                    var Ap = weapontype.attackPower;
                    Health health;
                    PlayerCombat combat;
                    GameObject player = target.gameObject;
                    if (shielded)
                    {
                        health = target.gameObject.GetComponentInParent<Health>();
                        combat = target.gameObject.GetComponentInParent<PlayerCombat>();
                        //yandan ataklar için
                        Ap = combat != null & combat.defending ? Ap - (Ap / 100 * target.gameObject.GetComponentInParent<PlayerCombat>().equippedshield?.damageReduction ?? 0) : Ap;
                    }
                    else { health = target.gameObject.GetComponent<Health>(); }
                    health.TakeDamage(Ap);
                    //TODO: Bir ara düzelt. burda hata verme ve element uygulamama ihtimali var shield durumunda
                    combatMethods.Elemental(player, weapontype);

                }
                else if (target.gameObject.tag == "Projectile" && target.transform.parent != parent)
                {
                    print("3");
                    collided = true;
                }
                else if (target.gameObject.tag == "Environment") { print("4"); collided = true; }
                else
                {
                    print("5");
                    return;
                }
                StartCoroutine(Destroyer(target.gameObject));
            }
        }

        private void OnDestroy()
        {
           
        }

        private void OnDisable()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Destroyer(gameObject));
            }

        }


        //void AOEEffect()
        //{
        //    var objects = Physics.OverlapSphere(transform.position, weapontype.attackradius).ToList();
        //    var enemies = objects.Where(a => a.tag == "Enemy").ToArray();
        //    if (enemies.Length > 0)
        //    {
        //        foreach (var item in enemies)
        //        {
        //            var Ap = weapontype.attackPower;
        //            item.GetComponent<Health>()?.TakeDamage(weapontype.attackPower);
        //            //try elemental
        //            Elemental(item.gameObject);
        //            KnockBack(item.gameObject);
        //        }

        //    }
        //}
        //void KnockBack(GameObject item)
        //{
        //    if (weapontype.knockBack && item.gameObject.tag != "Player")
        //    {
        //        Vector3 moveDirection = item.transform.position - transform.position;
        //        //var direction = item.transform.forward.normalized;
        //        //item.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 500f);
        //        moveDirection.y = 1.5f;
        //        StartCoroutine(item.GetComponent<EnemyMovement>().KnockBack(moveDirection.normalized * weapontype.knockBackPower, this.gameObject));
        //    }
        //}
        //private void Elemental(GameObject item)
        //{
        //    //burası dıger attacklar ıcın daha generık olabılırdı.
        //    if (weapontype.elemental)
        //    {
        //        GameObject elementbase = Instantiate(weapontype.element);
        //        var element = elementbase.GetComponent<ElementBase>();
        //        element.ApplyElement(item.gameObject, weapontype.elementalDuration, weapontype.elementalAttackPower);
        //        Destroy(elementbase);
        //    }
        //}
    }

}