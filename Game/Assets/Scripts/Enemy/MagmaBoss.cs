using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventArgs;
using Scriptables;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class MagmaBoss : MonoBehaviour
{
    [SerializeField] private MagmaBossData m_bossData;
    [SerializeField] private BossAttackCollider m_jumpAttackCollider;
    [SerializeField] private BossAttackCollider m_swipeAttackCollider;
    [SerializeField] private BossAttackCollider m_punchAttackCollider;
    [SerializeField] private BossRoarAttackCollider m_roarAttackCollider;
    [SerializeField] private ParticleSystem m_roarParticles;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private PlayerHud m_playerHud;

    private Transform m_targetTransform;
    private NavMeshAgent m_navMeshAgent;
    private Coroutine m_attackCoroutine;
    private Animator m_animator;

    private bool m_isDead;
    private bool m_isInFight;
    private float m_currentJumpAttackCooldown;
    private float m_currentRoarAttackCooldown;
    private float m_currentAttackCooldown;
    
    private static WaitForSeconds s_delayDetection = new WaitForSeconds(0.5f);
    
    public ResourceController HealthController { get; private set; }
    public int Damage => this.m_bossData.AttackDamage;

    public string Name => this.m_bossData.Name;

    public MagmaBossData Data => this.m_bossData;

    public void StartFight()
    {
        this.m_isInFight = true;
        this.m_audioSource.PlayOneShot(this.m_bossData.EncounterSound);
        MusicPlayer.Instance.PlayInstant(this.m_bossData.BossIntro);
        MusicPlayer.Instance.QueueClip(this.m_bossData.BossTheme);
    }
    
    private void Awake()
    {
        this.HealthController = new ResourceController(this.m_bossData.HealthData);
        this.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;

        this.m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.m_navMeshAgent.speed = this.m_bossData.MovementSpeed;
        this.m_navMeshAgent.stoppingDistance = this.m_bossData.AttackRange;

        this.m_animator = this.GetComponent<Animator>();
        this.m_currentJumpAttackCooldown = this.m_bossData.InitialJumpAttackCooldown;
        this.m_currentRoarAttackCooldown = this.m_bossData.InitialRoarCooldown;

        this.m_roarAttackCollider.Damage = this.m_bossData.RoarDamage;
        this.m_jumpAttackCollider.Damage = this.m_bossData.JumpAttackDamage;
        this.m_swipeAttackCollider.Damage = this.m_bossData.SwipeDamage;
        this.m_punchAttackCollider.Damage = this.m_bossData.PunchDamage;

        this.m_audioSource = this.GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.m_targetTransform = PlayerController.Instance.transform;
    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        if (this.m_isDead)
            return;
        
        if (e.NewValue <= 0)
        {
            this.Die();
            this.m_playerHud.ShowWinScreen();
            MusicPlayer.Instance.PlayGameWon();
        }
    }

    private void Die()
    {
        this.m_isDead = true;
        if (this.m_attackCoroutine != null)
        {
            StopCoroutine(this.m_attackCoroutine);
            this.m_attackCoroutine = null;
        }

        this.m_jumpAttackCollider.gameObject.SetActive(false);
        this.m_roarAttackCollider.gameObject.SetActive(false);
        this.m_punchAttackCollider.gameObject.SetActive(false);
        this.m_swipeAttackCollider.gameObject.SetActive(false);
        this.m_roarParticles.Stop();

        this.m_animator.SetBool("IsIdle", false);
        this.m_animator.SetBool("IsRunning", false);
        this.m_animator.SetTrigger("Die");
        this.m_isDead = true;
    }

    private void Update()
    {
        if (this.m_isDead || !this.m_isInFight)
            return;

        this.RefreshCooldowns();
        if (this.m_attackCoroutine == null)
        {
            this.RotateTowardsTarget();
            this.ProcessDistanceToTarget();   
        }
    }

    private void LateUpdate()
    {
        if (this.m_isDead)
            return;
        
        var isRunning = this.m_navMeshAgent.velocity.sqrMagnitude > 0f;
        this.m_animator.SetBool("IsRunning", isRunning && this.m_attackCoroutine == null);
        this.m_animator.SetBool("IsIdle", !isRunning && this.m_attackCoroutine == null);
    }

    private void RefreshCooldowns()
    {
        this.m_currentJumpAttackCooldown -= Time.deltaTime;
        this.m_currentRoarAttackCooldown -= Time.deltaTime;
        this.m_currentAttackCooldown -= Time.deltaTime;
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
        if (distance <= this.m_bossData.AttackRange)
        {
            //this.m_isAttackingIdle = true;
            this.m_navMeshAgent.isStopped = true;
            if (this.m_attackCoroutine == null && this.m_currentAttackCooldown <= 0f)
                this.m_attackCoroutine = this.StartCoroutine(this.HandleAttacking());
            // this.m_animator.SetBool(s_isAttackingIdleHash, true);
        }
        else if(this.m_attackCoroutine == null)
        {
            this.m_navMeshAgent.isStopped = false;
            this.m_navMeshAgent.destination = PlayerController.Instance.transform.position;
        }
    }

    private IEnumerator HandleAttacking()
    {
        this.m_animator.SetBool("IsIdle", false);
        if (this.m_currentJumpAttackCooldown <= 0f)
        {
            yield return this.JumpAttack();
        }
        else if (this.m_currentRoarAttackCooldown <= 0f)
        {
            yield return this.RoarAttack();
        }
        else
        {
            var chance = UnityEngine.Random.Range(0f, 1f);
            if (chance <= 0.2f)
                yield return this.SwipePunchCombo();
            if (chance <= 0.4f)
                yield return this.Punch();
            else
                yield return this.Swipe();
        }
        
        this.m_animator.SetBool("IsIdle", true);
        this.m_attackCoroutine = null;
    }

    private IEnumerator JumpAttack()
    {
        this.m_currentAttackCooldown = this.m_bossData.GeneralAttackCooldown;
        this.m_currentJumpAttackCooldown = this.m_bossData.JumpAttackCooldown;
        this.m_animator.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(this.m_bossData.JumpAttackAnimation.length);
    }

    private IEnumerator RoarAttack()
    {
        this.m_currentAttackCooldown = this.m_bossData.GeneralAttackCooldown;
        this.m_currentRoarAttackCooldown = this.m_bossData.RoarCooldown;
        this.m_animator.SetTrigger("Roar");

        yield return new WaitForSeconds(this.m_bossData.RoarAttackAnimation.length);

    }

    private IEnumerator Swipe()
    {
        this.m_currentAttackCooldown = this.m_bossData.GeneralAttackCooldown;
        this.m_animator.SetTrigger("Swipe");
        yield return new WaitForSeconds(this.m_bossData.SwipeAttackAnimation.length);

    }

    private IEnumerator Punch()
    {
        this.m_currentAttackCooldown = this.m_bossData.GeneralAttackCooldown;
        this.m_animator.SetTrigger("Punch");
        yield return new WaitForSeconds(this.m_bossData.PunchAttackAnimation.length);
    }

    private IEnumerator SwipePunchCombo()
    {
        this.m_currentAttackCooldown = this.m_bossData.GeneralAttackCooldown;
        this.m_animator.SetTrigger("Combo");
        yield return new WaitForSeconds(this.m_bossData.PunchAttackAnimation.length + this.m_bossData.SwipeAttackAnimation.length);
    }

    public void ActivateJumpAttackCollider()
    {
        StartCoroutine(this.TriggerJumpAttackCollider());
    }

    private IEnumerator TriggerJumpAttackCollider()
    {
        this.m_audioSource.PlayOneShot(this.m_bossData.JumpAttackSound);
        this.m_jumpAttackCollider.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        this.m_jumpAttackCollider.gameObject.SetActive(false);
    }
    
    public void ActivatePunchAttackCollider()
    {
        StartCoroutine(this.TriggerPunchAttackCollider());
    }
    
    private IEnumerator TriggerPunchAttackCollider()
    {
        this.m_audioSource.PlayOneShot(this.m_bossData.PunchSound);
        this.m_punchAttackCollider.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        this.m_punchAttackCollider.gameObject.SetActive(false);
    }
    
    public void ActivateSwipeAttackCollider()
    {
        StartCoroutine(this.TriggerSwipeAttackCollider());
    }
    
    private IEnumerator TriggerSwipeAttackCollider()
    {
        this.m_audioSource.PlayOneShot(this.m_bossData.SwipeSound);
        this.m_swipeAttackCollider.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        this.m_swipeAttackCollider.gameObject.SetActive(false);
    }
    
    public void StartRoarAttack()
    {
        this.m_audioSource.PlayOneShot(this.m_bossData.RoarSound);
        this.m_roarAttackCollider.gameObject.SetActive(true);
        this.m_roarParticles.Play();
    }

    public void StopRoarAttack()
    {
        this.m_roarAttackCollider.gameObject.SetActive(false);
        this.m_roarParticles.Stop();
    }
}
