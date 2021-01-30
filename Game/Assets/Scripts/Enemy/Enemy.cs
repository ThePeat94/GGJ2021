using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventArgs;
using Scriptables;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData m_enemyData;
    [SerializeField] private Collider m_attackCollider;
    [SerializeField] private LayerMask m_masksToHit;
    
    private Transform m_targetTransform;
    private NavMeshAgent m_navMeshAgent;
    private Coroutine m_playerDetectionCoroutine;
    private Coroutine m_wanderCoroutine;
    private Coroutine m_attackCoroutine;
    private Animator m_animator;

    private bool m_isWandering;
    private bool m_isAttacking;
    private bool m_isIdle;
    private bool m_isFollowingPlayer;
    private bool m_isDead;
    
    private static WaitForSeconds s_delayDetection = new WaitForSeconds(0.5f);
    
    public ResourceController HealthController { get; private set; }

    public virtual void Attack()
    {
        var hit = Physics.OverlapBox(this.m_attackCollider.bounds.center, this.m_attackCollider.bounds.extents, Quaternion.LookRotation(this.transform.forward), this.m_masksToHit);
        if (hit.Length > 0)
        {
            hit.First().GetComponent<PlayerController>().HealthController.UseResource(this.m_enemyData.AttackDamage);
        }
    }

    private void Awake()
    {
        this.HealthController = new ResourceController(this.m_enemyData.HealthData);
        this.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;

        this.m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.m_navMeshAgent.speed = this.m_enemyData.MovementSpeed;
        this.m_navMeshAgent.stoppingDistance = this.m_enemyData.AttackRange;

        this.m_animator = this.GetComponent<Animator>();
    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        if (e.NewValue <= 0)
        {
            this.m_isDead = true;
            if (this.m_playerDetectionCoroutine != null)
                this.StopCoroutine(this.m_playerDetectionCoroutine);
            
            // Play die animation
        }
    }

    private void Start()
    {
        this.m_playerDetectionCoroutine = this.StartCoroutine(this.DetectPlayer());
    }

    private void Update()
    {
        if (this.m_isDead)
            return;

        if (this.m_targetTransform != null)
        {
            this.RotateTowardsTarget();
            this.ProcessDistanceToTarget();
        }
    }
    
    private void RotateTowardsTarget()
    {
        var lookPosition = this.m_targetTransform.position - this.transform.position;
        lookPosition.y = 0f;
        var rotation = Quaternion.LookRotation(lookPosition);
        this.transform.rotation = rotation;
    }
    
    private void ProcessDistanceToTarget()
    {
        var distance = Vector3.Distance(this.transform.position, this.m_targetTransform.position);
        Debug.Log(distance);
        if (distance > this.m_enemyData.DetectionRadius)
        {
            this.m_navMeshAgent.isStopped = true;
            this.m_targetTransform = null;
            this.m_navMeshAgent.stoppingDistance = 1f;
            if (this.m_attackCoroutine != null)
            {
                this.StopCoroutine(this.m_attackCoroutine);
                this.m_attackCoroutine = null;
            }

            // this.m_isAttackingIdle = false;
        }
        else if (distance <= this.m_enemyData.AttackRange)
        {
            //this.m_isAttackingIdle = true;
            this.m_navMeshAgent.isStopped = true;
            if (this.m_attackCoroutine == null)
                this.m_attackCoroutine = this.StartCoroutine(this.HandleAttacking());
            // this.m_animator.SetBool(s_isAttackingIdleHash, true);
        }
        else
        {
            this.m_navMeshAgent.isStopped = false;
            if (this.m_attackCoroutine != null)
            {
                this.StopCoroutine(this.m_attackCoroutine);
                this.m_attackCoroutine = null;
            }
            // this.m_isAttackingIdle = false;
        }
    }

    private IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return s_delayDetection;
            var hitColliders = Physics.OverlapSphere(this.transform.position, this.m_enemyData.DetectionRadius);
            var hitPlayer = hitColliders.FirstOrDefault(c => c.GetComponent<PlayerController>());
            if(hitPlayer != null)
            {
                this.m_targetTransform = hitPlayer.transform;
                this.m_navMeshAgent.destination = hitPlayer.transform.position;
                this.m_navMeshAgent.isStopped = false;
            }
        }
    }

    private IEnumerator HandleAttacking()
    {
        while (true)
        {
            // this.m_animator.SetTrigger("Attack");
            this.Attack();
            Debug.Log("ATTACK");
            yield return new WaitForSeconds(1f/this.m_enemyData.AttacksPerSecond);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, this.m_enemyData.DetectionRadius);
    }

    private void OnCollisionEnter(Collision other)
    {
        var arrow = other.gameObject.GetComponent<Arrow>();
        if (arrow != null)
        {
            this.HealthController.UseResource(arrow.Damage);
        }
    }
}
