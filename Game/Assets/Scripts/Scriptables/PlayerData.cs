using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data", order = 0)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private float m_aimingMovementSpeed;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_rotationSpeed;
        [SerializeField] private float m_reloadingDuration;
        [SerializeField] private ResourceData m_healthData;
        [SerializeField] private ResourceData m_arrowData;

        public float AimingMovementSpeed => this.m_aimingMovementSpeed;
        public float MovementSpeed => this.m_movementSpeed;
        public float RotationSpeed => this.m_rotationSpeed;
        public float ReloadingDuration => this.m_reloadingDuration;

        public ResourceData HealthData => this.m_healthData;
        public ResourceData ArrowData => this.m_arrowData;
    }
}