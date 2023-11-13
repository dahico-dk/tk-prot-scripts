using Core;
using Core.Armor;
using Core.Combat;
using Core.Movement.Enemy;
using Core.Ranged;
using Core.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossCombat : MonoBehaviour
{
    public WeaponBase equippedweapon;
    public ArmorBase equippedarmor;

    Transform rangedspawnpoint;
    Animator anim;
    RuntimeAnimatorController runtimeanim;
    private Rigidbody rb;
    LayerMask mask;
    float timeSinceLastAttacks = 0;
    [HideInInspector] public GameObject target = null;
    public bool targeted;
    private float firstspeed;
    private Health health;
    public GameObject[] summonMelee;
    public GameObject[] summonRanged;
    public GameObject undeadNecro;
    public bool summonTest = false;
    private MiniBossMovement movment;
    public bool pushed = false;
    public int howManySummon = 4;
    public int[] thresholds;
    private void Start()
    {
        anim = GetComponent<Animator>();
        runtimeanim = GetComponent<RuntimeAnimatorController>();
        rangedspawnpoint = FindSpawnPoint();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        movment = GetComponent<MiniBossMovement>();
        StartCoroutine("UltiCheck");
    }
    private void Update()
    {
        if (target != null)
        {
            Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
            
            if (pushed)
            {
                var enemycount = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemycount.Length == 1)
                {
                    anim.SetTrigger("GoNormal");
                    EnableAura(false);
                    movment.move = true;
                    pushed = false;
                }

            }
        }
    }
    private IEnumerator UltiCheck()
    {
        Debug.Log("started");
        for (int i = 0; i < thresholds.Length; i++)
        {
            Debug.Log("waiting for threshold " + thresholds[i]);
            yield return new WaitUntil(() => health.HealthCheck(thresholds[i]));
            anim.SetTrigger("GoAway");
        }
        yield return null;
    }
    private void PushTargetBack()
    {
        target.GetComponent<Tester>().GoAway(transform.position);
        //var rbtarget = target.GetComponent<Rigidbody>();
        //var collider = target.GetComponent<Collider>();
        //if (collider!=null)
        //{
        //    collider.gameObject.SetActive(false);
        //}
        ////bool iskinematic = rbtarget.isKinematic;
        ////bool usegravity = rbtarget.useGravity;
        ////rbtarget.isKinematic = false;
        ////rbtarget.useGravity = true;
        //rbtarget.AddForce((transform.forward*50)+ transform.up, ForceMode.Impulse);
        EnableAura(true);
        movment.move = false;

        GetComponent<Collider>().gameObject.SetActive(true);

    }
    private void SummonEnemies()
    {
        for (int i = 0; i < howManySummon; i++)
        {
            var enemytosummon = UnityEngine.Random.Range(0, 100) < 50 ? summonMelee[UnityEngine.Random.Range(0, summonMelee.Length - 1)] : summonRanged[UnityEngine.Random.Range(0, summonRanged.Length - 1)];
            var newloc = new Vector3(transform.position.x + UnityEngine.Random.Range(-25, 25), -5, target.transform.position.z + UnityEngine.Random.Range(0, 25));
            var inst = Instantiate(enemytosummon, newloc, Quaternion.identity);
            inst.GetComponent<EnemyMovement>().summoned = true;
            var anim = inst.GetComponent<Animator>();
            anim.SetTrigger("Summoned");
        }
        pushed = true;

    }
    public void TriggerAttack(GameObject _target)
    {
        target = _target;
        timeSinceLastAttacks += Time.deltaTime;
        if (timeSinceLastAttacks > equippedweapon.attackSpeed)
        {
            timeSinceLastAttacks = 0;
            anim.SetTrigger("Attack");
        }
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
    Transform EnableAura(bool enable)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Aura")
            {
                child.gameObject.SetActive(enable);
            }
        }

        return null;
    }
    public void AttackTarget()
    {
        rb.isKinematic = true;
        //targeter = target;
        transform.LookAt(target.transform);
        timeSinceLastAttacks = 0;
        if (equippedweapon.melee)
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
        else
        {
            var wep = (Core.Weapons.Ranged)equippedweapon;
            //Vector3 relativePos = target.transform.position - transform.position;
            //transform.rotation = Quaternion.LookRotation(relativePos);
            transform.LookAt(target.transform);
            GameObject projectile = Instantiate(wep.rangedprefab, rangedspawnpoint.position, rangedspawnpoint.rotation);
            var proj = projectile.GetComponent<Projectile>();
            proj.parent = this.gameObject;
            proj.weapontype = (Core.Weapons.Ranged)equippedweapon;
        }
        rb.isKinematic = false;
    }
}
