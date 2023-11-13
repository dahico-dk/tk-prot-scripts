using Core.Animation;
using Core.Combat.AI;
using Core.Combat.Enemy;
using Core.Movement.Enemy;
using Core.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Core
{

    public class ObjectRef
    {
        public string tag { get; set; }
        public Vector3 lastLocation { get; set; }
        public float DropRateBonus { get; set; }
       
    }   
    internal class Health : MonoBehaviour
    {
        public float healthpoints;
        public float maxhealtpoints;
        public bool elemented;
        [HideInInspector]bool godMode;
        [SerializeField] bool dissolve;      
        [SerializeField] Image HealthBar;
        [SerializeField] Text HpText;
        public delegate void SomeBodyDiedEventHandler(ObjectRef refe);
        public event SomeBodyDiedEventHandler DieMOFO;
        private Animator anim;
        private bool died;
        private PoolManager poolmanager;

        private void OnEnable()
        {
            elemented = false;
            godMode = false;
            died = false;
        }
        private void Awake()
        {
            //healthpoints = 100;
            //maxhealtpoints = 100;
            elemented = false;
            godMode = false;          
            died = false;
        }
        private void Start()
        {
            anim = GetComponent<Animator>();
            var pmobj = GameObject.FindGameObjectWithTag("PoolManager");
            poolmanager = pmobj.GetComponent<PoolManager>();
            //if (HealthBar==null)
            //{
            //    foreach (Transform child in transform)
            //    {
            //        if (child.tag == "HealthBar")
            //        {
            //            HealthBar =child.GetComponent<Image>();
            //        }
            //    }
            //}
            //DieMOFO += DieMofoDie;
        }           
        private void Update()
        {
            //take damage acts as check damage
            TakeDamage(0,this.gameObject,false);
        }
        private void FixedUpdate()
        {
            if (HealthBar)
                HealthBar.fillAmount = healthpoints / 100 <= 0 ? 0 : healthpoints / 100;

            if (HpText)
                HpText.text = healthpoints + " / " + maxhealtpoints;
        }
        public void TakeDamage(float damage, GameObject caller=null,bool animate=true)
        {
            if (!godMode&&!died)
            {
                healthpoints = (healthpoints - damage) > 0 ? (healthpoints - damage) : 0;
                if (damage>0)
                {
                    PopupDamage(damage);
                }               
                if (healthpoints <= 0)
                {
                    died = true;
                    if (caller!=null && caller.GetComponent<AICombat>()!=null)
                    {
                        caller.GetComponent<AICombat>().Invoke("KingIsDead", 0);
                    }
                    SomeBodyDied();
                }
                else
                {
                    if (anim != null&&animate)
                    {
                        if (gameObject.tag == "Enemy")
                        {
                            gameObject.GetComponent<EnemyMovement>().move = false;
                        }
                        anim.SetTrigger("Take Damage");                      
                    }
                }
            }         
        }
        private void PopupDamage(float damage)
        {
            var popup = poolmanager.GetPooledPopUp();
            var popmanager = popup.GetComponent<DamagePopup>();
            popup.transform.position = transform.position + Vector3.up;
            popmanager.Setup(Convert.ToInt32(damage), transform.position + Vector3.up);
        }
        public void DamageAnimationEnd()
        {
            gameObject.GetComponent<EnemyMovement>().move = true;
        }
        void DieEffect()
        {
            SetObjectComponents(false);
            GetComponentsInChildren<ParticleSystem>().ToList().ForEach(item =>
            {
                item.Stop();
            });
            GetComponentsInChildren<Light>().ToList().ForEach(item =>
            {
                item.range = item.intensity = 0;
            });
            var enanim = GetComponent<EnemyAnimations>();
            if (dissolve)
            {
                enanim.dissolve = true;
            }
            else
            {
                enanim.SmokeEffect();
            }
           
            
        }    
        protected virtual void SomeBodyDied()
        {          
            anim.SetTrigger("Die");
            var agent = GetComponent<NavMeshAgent>();
            if (agent)
                agent.isStopped = true;
            SetObjectComponents(false);
            DieMOFO(new ObjectRef()
            {
                tag = gameObject.tag,
                DropRateBonus = 0,
                lastLocation = transform.position
            });
        }
        public void SetObjectComponents(bool v)
        {
            gameObject.GetComponent<EnemyMovement>().enabled = v;
            gameObject.GetComponent<EnemyCombat>().enabled = v;
            gameObject.GetComponent<Rigidbody>().isKinematic = !v;
            gameObject.GetComponent<BoxCollider>().enabled = v;
            gameObject.GetComponentInChildren<Light>().enabled = v;
            gameObject.GetComponent<NavMeshAgent>().radius = v?0:1f;
            gameObject.GetComponent<NavMeshAgent>().enabled = v;
            foreach (Transform child in transform)
            {
                if (child.tag == "Canvas")
                {
                    child.gameObject.SetActive(v);
                }
            }
            if (v)
            {
                healthpoints = maxhealtpoints;
            }

        }
        public void Heal(float extrahealth)
        {
            healthpoints = (healthpoints + extrahealth) < maxhealtpoints ? (healthpoints + extrahealth) : maxhealtpoints;
        }
        public bool HealthCheck(float percent)
        {
            var min = (maxhealtpoints - (maxhealtpoints * (percent + 3)) / 100);
            var max = (maxhealtpoints - (maxhealtpoints * (percent - 10)) / 100);
            if (healthpoints < max && healthpoints >= min)
            {
                return true;
            }
            return false;
        }       
        public void SetGodMode(bool stat)
        {
            if (gameObject.tag == "Player")
            {
                godMode = stat;
            }
        }
        public bool CheckAlive()
        {
            return !died;
        }

    }
}
