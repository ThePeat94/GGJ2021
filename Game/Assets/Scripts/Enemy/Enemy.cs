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
    [SerializeField] private LayerMask m_wanderMasksToHit;
    
    private Transform m_targetTransform;
    private NavMeshAgent m_navMeshAgent;
    private Coroutine m_playerDetectionCoroutine;
    private Coroutine m_wanderCoroutine;
    private Coroutine m_attackCoroutine;
    private Animator m_animator;
    private Vector3 m_origin;

    private bool m_isWandering;
    private bool m_isAttacking;
    private bool m_isIdle;
    private bool m_isFollowingPlayer;
    private bool m_isDead;
    
    private static WaitForSeconds s_delayDetection = new WaitForSeconds(0.5f);
    
    public ResourceController HealthController { get; private set; }
    public int Damage => this.m_enemyData.AttackDamage;

    private void Awake()
    {
        this.HealthController = new ResourceController(this.m_enemyData.HealthData);
        this.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;

        this.m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.m_navMeshAgent.speed = this.m_enemyData.MovementSpeed;
        this.m_navMeshAgent.stoppingDistance = this.m_enemyData.AttackRange;

        this.m_animator = this.GetComponent<Animator>();
        this.m_origin = this.transform.position;
    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        if (e.NewValue <= 0)
        {
            this.m_isDead = true;
            if (this.m_playerDetectionCoroutine != null)
                this.StopCoroutine(this.m_playerDetectionCoroutine);
            
            this.StopWandering();
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
        else if(this.m_wanderCoroutine == null)
        {
            this.m_wanderCoroutine = StartCoroutine(this.Wander());
        }
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            var rndPoint = UnityEngine.Random.insideUnitSphere * this.m_enemyData.WanderRadius + this.m_origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(rndPoint, out navHit, this.m_enemyData.WanderRadius, this.m_wanderMasksToHit);
            this.m_navMeshAgent.destination = navHit.position;
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
        if (distance > this.m_enemyData.DetectionRadius)
        {
            this.m_navMeshAgent.destination = this.transform.position;
            this.m_targetTransform = null;
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
                this.StopWandering();
            }
        }
    }

    private IEnumerator HandleAttacking()
    {
        while (true)
        {
            // this.m_animator.SetTrigger("Attack");
            StartCoroutine(this.Attack());
            Debug.Log("ATTACK");
            yield return new WaitForSeconds(1f/this.m_enemyData.AttacksPerSecond);
        }
    }
    
    protected virtual IEnumerator Attack()
    {
        this.m_attackCollider.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        this.m_attackCollider.gameObject.SetActive(false);
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

    private void StopWandering()
    {
        if(this.m_wanderCoroutine != null)
        {
            StopCoroutine(this.m_wanderCoroutine);
            this.m_wanderCoroutine = null;
        }
    }
}
