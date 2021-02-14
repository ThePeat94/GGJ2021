using UnityEngine;

namespace Nidavellir.FoxIt
{
    public class AimCamera : MonoBehaviour
    {
        [SerializeField] private GameInputProcessor m_playerGameInput;
        [SerializeField] private float m_maxUpwardsAngle;
        [SerializeField] private float m_maxDownwardsAngle;
        [SerializeField] private float m_sensivity;

        private float m_currentXRotation;

        private void Update()
        {
            this.m_currentXRotation -= this.m_playerGameInput.MouseDelta.y * this.m_sensivity;
            this.m_currentXRotation = Mathf.Clamp(this.m_currentXRotation, this.m_maxDownwardsAngle, this.m_maxUpwardsAngle);
            this.transform.rotation = Quaternion.Euler(this.m_currentXRotation, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        }
    }
}