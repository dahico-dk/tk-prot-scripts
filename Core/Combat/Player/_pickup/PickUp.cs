using Core.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Combat.Player
{
    public class PickUp : MonoBehaviour
    {
        public PickUpType ptype;
        private GameObject target;
        private float totalTimeSpent;
        private bool applied = false;
        [SerializeField]ParticleSystem taken;
        private void Update()
        {
            totalTimeSpent += Time.deltaTime;
            if (totalTimeSpent > 120)
                StartCoroutine(Dest(0, true));
        }

        
        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.tag == "Player" || other.transform.parent.gameObject.tag == "Player")
            {
                //not cool
                target = GameObject.FindGameObjectWithTag("Player");
                ApplyPowerUp();
            }
        }
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.tag == "Player")
        //    {
        //        target = collision.gameObject;
        //        ApplyPowerUp();
        //    }
        //}
        void ApplyPowerUp()
        {          
            //foreach (Transform child in transform)
            //{
            //    if (child.name != "Taken" && child.transform.parent.name != "Taken")
            //    {
            //        Destroy(child);
            //    }
            //}
            if (!applied)
            {
                applied = true;
                switch (ptype)
                {
                    case PickUpType.HealthPickUp:
                        HealthPickUp(50);
                        break;
                    case PickUpType.HealthPickUpBig:
                        HealthPickUp(250);
                        break;
                    case PickUpType.SpeedBoost:
                        StartCoroutine(SpeedBoost(10));
                        break;
                    case PickUpType.DamageBoost:
                        StartCoroutine(DamageBoost(15));
                        break;
                    case PickUpType.DamageAll:
                        DamageAll(20);
                        break;
                    case PickUpType.UltiCounterReset:
                        //WIP
                        break;
                    case PickUpType.Invincibility:
                        StartCoroutine(Invincibility(15));
                        break;
                    default:
                        break;
                }
            }

        }       
        private IEnumerator Invincibility(float duration)
        {
            var health = target.GetComponent<Health>();
            health.SetGodMode(true);
            yield return new WaitForSeconds(duration);
            health.SetGodMode(false);
            StartCoroutine(Dest(duration, true));
            yield return null;
        }
        void HealthPickUp(float amount)
        {

            target.GetComponent<Health>().Heal(amount);
            StartCoroutine(Dest(0, true));
        }
        IEnumerator SpeedBoost(float duration)
        {
            var movement = target.GetComponent<PlayerMovement>();
            var oldspeed = movement.speed;
            movement.speed = oldspeed * 3;
            StartCoroutine(Dest(duration, true));
            yield return new WaitForSeconds(duration);
            movement.speed = oldspeed;
           
            yield return null;
        }
        IEnumerator DamageBoost(float duration)
        {
            var combat = target.GetComponent<PlayerCombat>();
            var oldweapon = target.GetComponent<PlayerCombat>().equippedweapon;
            var changer = target.GetComponent<PlayerCombat>().equippedweapon;
            changer.attackSpeed = changer.attackSpeed / 2;
            changer.attackPower = changer.attackPower * 2;
            combat.equippedweapon = changer;
            StartCoroutine(Dest(duration,true));
            yield return new WaitForSeconds(duration);
            combat.equippedweapon = oldweapon;           
            yield return null;
        }
        private IEnumerator Dest(float duration,bool anim = true)
        {
            transform.GetComponent<Animator>().enabled = false;
            transform.GetComponent<Collider>().enabled = false;
            GetComponentsInChildren<MeshRenderer>().ToList().ForEach(item =>
            {
                item.enabled = false;
            });
            GetComponentsInChildren<ParticleSystem>().ToList().ForEach(item =>
            {
                item.Stop();
            });


            if (anim)
            {
                foreach (Transform child in transform)
                {
                    if (child != taken && child.transform.parent != taken)
                    {
                        Destroy(child);
                    }
                }
                taken.Play();
            }
            yield return new WaitForSeconds(duration);
            Destroy(this.gameObject);
        }
        void DamageAll(float range)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var item in enemies)
            {
                var hlth = item.GetComponent<Health>();
                item.GetComponent<Health>().TakeDamage(((hlth.healthpoints * range) / 100));
                //buraya efekt gelmeli
            }
            StartCoroutine(Dest(0, true));
        }

    }
}
