using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(menuName = "Data/Enemy Data", order = 0)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_detectionRadius;
        [SerializeField] private int m_attackDamage;
        [SerializeField] private float m_wanderRadius;
        [SerializeField] private float m_attackRange;
        [SerializeField] private ResourceData m_healthData;
        [SerializeField] private AnimationClip m_attackClip;
        [SerializeField] private AudioClip m_hitByArrowSound;
        [SerializeField] private AudioClip m_attackSound;
        
        public float MovementSpeed => this.m_movementSpeed;
        public float WanderRadius => this.m_wanderRadius;
        public AnimationClip AttackClip => this.m_attackClip;
        public float AttackRange => this.m_attackRange;
        public float DetectionRadius => this.m_detectionRadius;

        public int AttackDamage => this.m_attackDamage;

        public ResourceData HealthData => this.m_healthData;
        public AudioClip HitByArrowSound => this.m_hitByArrowSound;
        public AudioClip AttackSound => this.m_attackSound;

    }
}