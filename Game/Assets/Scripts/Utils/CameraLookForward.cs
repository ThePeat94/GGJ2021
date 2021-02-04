using UnityEngine;

namespace Nidavellir.FoxIt.Utils
{
    public class CameraLookForward : MonoBehaviour
    {
        private Transform m_camera;
        
        private void Awake()
        {
            this.m_camera = Camera.main.transform;
        }

        private void Update()
        {
            var resetRotation = this.transform.rotation.eulerAngles;
            resetRotation.x = 0f;
            this.transform.rotation = Quaternion.Euler(resetRotation);
        }
    }
}