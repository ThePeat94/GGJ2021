using System.Collections;
using System.Linq;
using Nidavellir.FoxIt.Enemy.Colliders;
using Nidavellir.FoxIt.Enemy.Lord_Magma;
using Nidavellir.FoxIt.EventArgs;
using Nidavellir.FoxIt.Scriptables;
using UnityEngine;
using UnityEngine.AI;

namespace Nidavellir.FoxIt.Enemy.Turtoise_McHog
{
    public class TurtoiseMcHog : MonoBehaviour
    {
        private static readonly WaitForSeconds s_delayDetection = new WaitForSeconds(0.5f);
        [SerializeField] private EnemyData m_enemyData;
        [SerializeField] private AttackCollider m_attackCollider;
        [SerializeField] private LayerMask m_wanderMasksToHit;
        private Animator m_animator;
        private Coroutine m_attackCoroutine;

        private bool m_isDead;
        private NavMeshAgent m_navMeshAgent;
        private Vector3 m_origin;
        private Coroutine m_playerDetectionCoroutine;

        private Transform m_targetTransform;
        private Coroutine m_wanderCoroutine;
        public ResourceController HealthController { get; private set; }

        private void Awake()
        {
            this.HealthController = new ResourceController(this.m_enemyData.HealthData);
            this.HealthController.ResourceValueChanged += this.HealthControllerOnResourceValueChanged;

            this.m_navMeshAgent = this.GetComponent<NavMeshAgent>();
            this.m_navMeshAgent.speed = this.m_enemyData.MovementSpeed;
            this.m_navMeshAgent.stoppingDistance = this.m_enemyData.AttackRange;

            this.m_animator = this.GetComponent<Animator>();
            this.m_origin = this.transform.position;

            this.m_attackCollider.Damage = this.m_enemyData.AttackDamage;
            this.m_attackCollider.HitPlayerClip = this.m_enemyData.AttackSound;
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
            else if (this.m_wanderCoroutine == null && this.m_attackCoroutine == null)
            {
                this.m_wanderCoroutine = this.StartCoroutine(this.Wander());
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            var arrow = other.gameObject.GetComponent<Arrow>();
            if (arrow != null)
            {
                CameraSoundPlayer.Instance.PlayClipAtCam(this.m_enemyData.HitByArrowSound);
                this.HealthController.UseResource(arrow.Damage);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.m_enemyData.DetectionRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, this.m_enemyData.WanderRadius);
        }

        public void ActivateAttackCollider()
        {
            this.StartCoroutine(this.TriggerAttackCollider());
        }

        private IEnumerator DetectPlayer()
        {
            while (true)
            {
                yield return s_delayDetection;
                var hitColliders = Physics.OverlapSphere(this.transform.position, this.m_enemyData.DetectionRadius);
                var hitPlayer = hitColliders.FirstOrDefault(c => c.GetComponent<PlayerController>());
                if (hitPlayer != null)
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
            this.m_animator.SetBool("IsWalking", false);
            this.m_animator.SetTrigger("Attack");
            yield return new WaitForSeconds(this.m_enemyData.AttackClip.length + 1f);
            this.m_attackCoroutine = null;
        }

        private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEvent e)
        {
            if (this.m_isDead)
                return;

            if (e.NewValue <= 0)
            {
                this.m_isDead = true;
                if (this.m_playerDetectionCoroutine != null)
                {
                    this.StopCoroutine(this.m_playerDetectionCoroutine);
                    this.m_playerDetectionCoroutine = null;
                }

                if (this.m_attackCoroutine != null)
                {
                    this.StopCoroutine(this.m_attackCoroutine);
                    this.m_attackCoroutine = null;
                }

                this.StopWandering();

                this.m_navMeshAgent.destination = this.transform.position;
                this.m_navMeshAgent.isStopped = true;

                this.m_animator.SetBool("IsWalking", false);
                this.m_animator.ResetTrigger("Attack");
                this.m_animator.SetTrigger("Die");
                Destroy(this.gameObject, 5f);
            }
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
            }
            else if (distance <= this.m_enemyData.AttackRange)
            {
                this.StopWandering();
                this.m_navMeshAgent.isStopped = true;
                if (this.m_attackCoroutine == null)
                    this.m_attackCoroutine = this.StartCoroutine(this.HandleAttacking());
            }
            else
            {
                this.m_navMeshAgent.isStopped = false;
                this.StopAttacking();
            }
        }

        private void RotateTowardsTarget()
        {
            var lookPosition = this.m_targetTransform.position - this.transform.position;
            lookPosition.y = 0f;
            var rotation = Quaternion.LookRotation(lookPosition);
            this.transform.rotation = rotation;
        }

        private void StopAttacking()
        {
            if (this.m_attackCoroutine != null)
            {
                this.StopCoroutine(this.m_attackCoroutine);
                this.m_attackCoroutine = null;
                this.m_animator.ResetTrigger("Attack");
            }
        }

        private void StopWandering()
        {
            if (this.m_wanderCoroutine != null)
            {
                this.StopCoroutine(this.m_wanderCoroutine);
                this.m_wanderCoroutine = null;
            }
        }

        private IEnumerator TriggerAttackCollider()
        {
            this.m_attackCollider.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            this.m_attackCollider.gameObject.SetActive(false);
        }

        private IEnumerator Wander()
        {
            while (true)
            {
                var randomWaitTime = Random.Range(1.5f, 7f);
                this.m_animator.SetBool("IsWalking", true);
                yield return new WaitForSeconds(randomWaitTime);
                var rndPoint = Random.insideUnitSphere * this.m_enemyData.WanderRadius + this.m_origin;
                NavMeshHit navHit;
                NavMesh.SamplePosition(rndPoint, out navHit, this.m_enemyData.WanderRadius, this.m_wanderMasksToHit);
                this.m_navMeshAgent.destination = navHit.position;
            }
        }
    }
}