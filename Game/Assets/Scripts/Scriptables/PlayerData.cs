using UnityEngine;

namespace Nidavellir.FoxIt.Scriptables
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data", order = 0)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private float m_aimingMovementSpeed;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_rotationSpeed;
        [SerializeField] private float m_reloadingDuration;
        [SerializeField] private ResourceData m_healthData;
        [SerializeField] private ResourceData m_quiverData;
        [SerializeField] private int m_attackDamage;
        [SerializeField] private float m_shootCooldown;
        [SerializeField] private float m_shootForce;
        [SerializeField] private float m_mouseSensivity;
        [SerializeField] private AudioClip m_arrowShootSound;
        [SerializeField] private AudioClip m_reloadSound;
        [SerializeField] private AudioClip m_gameOverTheme;
        [SerializeField] private AudioClip m_collectFoxSound;

        public float MouseSensivity => this.m_mouseSensivity;
        public float AimingMovementSpeed => this.m_aimingMovementSpeed;
        public float MovementSpeed => this.m_movementSpeed;
        public float RotationSpeed => this.m_rotationSpeed;
        public float ReloadingDuration => this.m_reloadingDuration;

        public ResourceData HealthData => this.m_healthData;
        public ResourceData QuiverData => this.m_quiverData;
        public int AttackDamage => this.m_attackDamage;
        public float ShootCooldown => this.m_shootCooldown;
        public float ShootForce => this.m_shootForce;

        public AudioClip ArrowShootSound => this.m_arrowShootSound;
        public AudioClip ReloadSound => this.m_reloadSound;
        public AudioClip GameOverTheme => this.m_gameOverTheme;
        public AudioClip CollectFoxSound => this.m_collectFoxSound;
    }
}