using UnityEngine;

namespace Nidavellir.FoxIt.Scriptables
{
    [CreateAssetMenu(fileName = "Magma Boss Data", menuName = "Data/Bosses/Magma", order = 0)]
    public class MagmaBossData : ScriptableObject
    {
        [SerializeField] private string m_name;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private int m_attackDamage;
        [SerializeField] private float m_attackRange;
        [SerializeField] private ResourceData m_healthData;
        [SerializeField] private float m_roarCooldown;
        [SerializeField] private float m_jumpAttackCooldown;
        [SerializeField] private float m_generalAttackCooldown;
        [SerializeField] private int m_jumpAttackDamage;
        [SerializeField] private int m_roarDamage;
        [SerializeField] private int m_swipeDamage;
        [SerializeField] private int m_punchDamage;
        [SerializeField] private float m_initialRoarCooldown;
        [SerializeField] private float m_initialJumpAttackCooldown;
        [SerializeField] private AnimationClip m_jumpAttackAnimation;
        [SerializeField] private AnimationClip m_roarAttackAnimation;
        [SerializeField] private AnimationClip m_swipeAttackAnimation;
        [SerializeField] private AnimationClip m_punchAttackAnimation;
        [SerializeField] private AudioClip m_roarSound;
        [SerializeField] private AudioClip m_punchSound;
        [SerializeField] private AudioClip m_swipeSound;
        [SerializeField] private AudioClip m_jumpAttackSound;
        [SerializeField] private AudioClip m_encounterSound;
        [SerializeField] private AudioClip m_bossIntro;
        [SerializeField] private AudioClip m_bossTheme;
        [SerializeField] private AudioClip m_hitByArrowSound;
        [SerializeField] private AudioClip m_deathSound;

        public string Name => this.m_name;
        public float MovementSpeed => this.m_movementSpeed;
        public int AttackDamage => this.m_attackDamage;
        public float AttackRange => this.m_attackRange;
        public ResourceData HealthData => this.m_healthData;
        public float RoarCooldown => this.m_roarCooldown;
        public float JumpAttackCooldown => this.m_jumpAttackCooldown;
        public float GeneralAttackCooldown => this.m_generalAttackCooldown;
        public int JumpAttackDamage => this.m_jumpAttackDamage;
        public int RoarDamage => this.m_roarDamage;
        public int SwipeDamage => this.m_swipeDamage;
        public int PunchDamage => this.m_punchDamage;
        public float InitialRoarCooldown => this.m_initialRoarCooldown;
        public float InitialJumpAttackCooldown => this.m_initialJumpAttackCooldown;
        public AnimationClip JumpAttackAnimation => this.m_jumpAttackAnimation;
        public AnimationClip RoarAttackAnimation => this.m_roarAttackAnimation;
        public AnimationClip SwipeAttackAnimation => this.m_swipeAttackAnimation;
        public AnimationClip PunchAttackAnimation => this.m_punchAttackAnimation;

        public AudioClip RoarSound => this.m_roarSound;
        public AudioClip PunchSound => this.m_punchSound;
        public AudioClip SwipeSound => this.m_swipeSound;
        public AudioClip JumpAttackSound => this.m_jumpAttackSound;
        public AudioClip EncounterSound => this.m_encounterSound;
        public AudioClip BossIntro => this.m_bossIntro;
        public AudioClip BossTheme => this.m_bossTheme;
        public AudioClip HitByArrowSound => this.m_hitByArrowSound;
        public AudioClip DeathSound => this.m_deathSound;
    }
}