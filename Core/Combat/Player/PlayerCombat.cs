using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Core.Weapons;
using Core.Armor;
using Core.Ranged;
using Core.Shield;
using Core.Movement;
using Core.Movement.Enemy;
using Core.Animation;

namespace Core.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        float timeCount = 0.0f;
        public bool defending = false;
        Vector3 oldpos;
        #region equipment
        public WeaponBase[] elementalWeapons;
        public WeaponBase defaultWeapon;
        public WeaponBase equippedweapon;
        public ArmorBase equippedarmor;
        public ShieldBase equippedshield;
        public GameObject shieldPrefab;        
        public ParticleSystem shieldParticle;
        public ParticleSystem summonParticle;
        public Transform rangedspawnpoint;
        #endregion
        private CombatMethods combatMethods;
        private PlayerAnimate animation;
        float timeSinceLastAttack = 100;
        float normalSpeed;
        int attackCounter = 0;
        public float AttackSlowDownRate = 30;
        float switchCounter = 3f;
        float lastSwitch = 100;
        PlayerMovement mover;
        bool OpenEffectTriggered = false;
        bool CloseEffectTriggered = true;
        bool switching = false;     
        private PoolManager poolmanager;
        private void Start()
        {
            equippedweapon = defaultWeapon;
            oldpos = shieldPrefab.transform.localPosition;
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
             mover = GetComponent<PlayerMovement>();
            normalSpeed = mover.speed;
            combatMethods = GetComponent<CombatMethods>();
            animation = GetComponent<PlayerAnimate>();
            SwitchWeaponEffect(equippedweapon.EffectName);
            shieldParticle.Stop();
        }
        private void ShieldEffect(bool v)
        {
            if (v)
            {
                summonParticle.Play();
                shieldParticle.Play();
            }
            else
            {
                summonParticle.Play();
                shieldParticle.Stop();
            }
        }
        private void SwitchWeapon()
        {
            var current = Array.FindIndex(elementalWeapons, 0, elementalWeapons.Length, a => a == equippedweapon);
            //var index= current.
            equippedweapon = elementalWeapons[current + 1 < elementalWeapons.Length ? current + 1 : 0];
            //switch to effect         
            SwitchWeaponEffect(equippedweapon.EffectName);
            switching = false;
        }
        private void SwitchWeaponEffect(string effectName)
        {
            var iterator = GameObject.FindGameObjectsWithTag("SwordEffect");
            foreach (var child in iterator)
            {
                if (child.gameObject.name == effectName)
                {
                    child.GetComponent<ParticleSystem>().Play();
                    child.GetComponentInChildren<Light>().enabled = true;
                }
                else
                {

                    child.GetComponent<ParticleSystem>().Stop();
                    child.GetComponentInChildren<Light>().enabled = false;
                }
            }

        }
        float FindAngle(Transform target)
        {
            Vector3 targetDir = target.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            return angle;
        }
        private List<GameObject> FindEnemies()
        {
            var newpos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            LayerMask enemyMask = LayerMask.GetMask("Enemy");
            //List<Collider> hitColliders = Physics.OverlapSphere(newpos, radius).ToList();
            List<GameObject> objef = new List<GameObject>();
            var things = Physics.OverlapSphere(newpos, equippedweapon.attackradius, enemyMask);
            foreach (var item in things)
            {
                //infront of player
                var heading = item.transform.position - transform.position;
                var dot = Vector3.Dot(heading, transform.forward);
                var angle = FindAngle(item.transform);
                if (angle <= 90)
                {
                    //&& item.gameObject.tag == "Enemy" layermaskle bırlıkte gerek kalmadı
                    //print(item.name + " angle:" + angle);
                    objef.Add(item.gameObject);
                }
                //left radius


                //right radius
            }

            return objef;

        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            lastSwitch += Time.deltaTime;                      
        }      
        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0) && (timeSinceLastAttack > equippedweapon.attackSpeed) && !defending&&!switching)
            {
                attackCounter = (attackCounter < 5) ? attackCounter + 1 : 1;
                animation.AttackAnimation(attackCounter);
                mover.speed = normalSpeed - ((normalSpeed / 100) * AttackSlowDownRate);             
                timeSinceLastAttack = 0;
            }
            if (Input.GetMouseButton(1))
            {
                DefenceBehaviour(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                DefenceBehaviour(false);
            }
            if (Input.GetKeyDown(KeyCode.Tab) && lastSwitch > switchCounter && !defending)
            {
                switching = true;
                animation.SwitchAnimation();
            }

            mover.speed = normalSpeed;
        }
        void ShieldRotationalMovement(bool press)
        {
            //not so rotational mofo
            if (press)
            {

                shieldPrefab.transform.localPosition = new Vector3(0, oldpos.y, 1);
            }
            else
            {
                shieldPrefab.transform.localPosition = oldpos;
            }

        }
        private void DefenceBehaviour(bool def)
        {
            var mover = GetComponent<PlayerMovement>();
            Quaternion ninty = Quaternion.Euler(0, 90, 0);
            Quaternion zeroy = Quaternion.Euler(0, 0, 0);
            timeCount = 0.0f;
            //defending = true;
            //mover.speed = normalSpeed - ((normalSpeed / 100) * equippedshield.slowDownRate);
            if (def)
            {
                defending = true;
                mover.speed = normalSpeed - ((normalSpeed / 100) * equippedshield.slowDownRate);
                if (shieldPrefab.transform.localEulerAngles.y < 90)
                {
                    shieldPrefab.transform.localEulerAngles = new Vector3(0, 90, 0);
                }
                if (!OpenEffectTriggered)
                {
                    OpenEffectTriggered = true;
                    CloseEffectTriggered = false;
                    ShieldEffect(true);
                }
                ShieldRotationalMovement(true);
            }
            else
            {
                defending = false;
                mover.speed = normalSpeed;
                if (shieldPrefab.transform.localEulerAngles.y >= 90)
                {
                    shieldPrefab.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                if (!CloseEffectTriggered)
                {
                    OpenEffectTriggered = false;
                    CloseEffectTriggered = true;
                    ShieldEffect(false);
                }
                ShieldRotationalMovement(false);
            }
            animation.Block(defending);
        }
        private void MeleeAttackBehaviour()
        {
            var enemies = FindEnemies();
            foreach (var item in enemies)
            {
                //print("die => " + item.name);
                //KnockBackEnemy(item);
                CreateHitEffect(item);
                //combatMethods.KnockBack(item, equippedweapon, this.transform);
                combatMethods.Elemental(item, equippedweapon);
                combatMethods.TranslateDamage(item, equippedweapon.attackPower);
            }
        }
        private void CreateHitEffect(GameObject item)
        {
            var mel = (Melee)equippedweapon;
            var hiteffect = poolmanager.GetPooledHitEffect(mel.typer);
            hiteffect.SetActive(true);
            hiteffect.transform.localPosition = new Vector3(0, item.transform.lossyScale.y/2,0);
            hiteffect.GetComponent<AutoDestroy>().StartCountDown(3, hiteffect);           
        }
        private void RangedAttackBehaviour()
        {
            var wep = (Core.Weapons.Ranged)equippedweapon;
            var typer = wep.rangedprefab.GetComponent<EnumHolder>().projType;
            GameObject projectile = poolmanager.GetPooledProjectile(typer);
            projectile.transform.position = rangedspawnpoint.position;
            projectile.transform.rotation = rangedspawnpoint.rotation;          
            var proj = projectile.GetComponent<Projectile>();
            
            proj.parent = gameObject;
            proj.weapontype = (Core.Weapons.Ranged)equippedweapon;
            projectile.GetComponent<ParticleSystem>().Play();
            projectile.GetComponent<Light>().enabled = true;
        }



    }
}