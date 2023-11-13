using Core.Combat.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private GameObject target;
    private Animator animator;
    public bool move = false;
    MiniBossCombat combat;

    private bool randomLocAssigned = false;
    float attackRange;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        target = GameObject.FindGameObjectWithTag("Player");
        combat = GetComponent<MiniBossCombat>();
        attackRange = combat.equippedweapon.attackradius;
        animator = GetComponent<Animator>();
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
    void FixedUpdate()
    {
        try
        {
            if (move && target)
            {                
                if (EnemyInRange())
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
        catch (System.Exception ex)
        {
            print(this.name);
            throw;
        }
    }
    internal bool EnemyInRange()
    {      
        if (Vector3.Distance(target.transform.position, transform.position) >= attackRange)
        {
            return true;
        }
        return false;
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
    IEnumerator DisasterScenerio()
    {
        yield return new WaitForSeconds(3.5f);
        if (rb.isKinematic || !agent.enabled || !move)
        {
            Finisher();
            yield return null;
        }
    }
    void Finisher()
    {
        agent.enabled = true;
        move = true;
        rb.isKinematic = false;

    }




}
