using UnityEngine;

namespace Nidavellir.FoxIt.Enemy.Colliders
{
    public class AttackCollider : MonoBehaviour
    {
        public int Damage { get; set; }
        public AudioClip HitPlayerClip { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            var hitPlayer = other.GetComponent<PlayerController>();
            if (hitPlayer != null)
            {
                hitPlayer.HealthController.UseResource(this.Damage);
                if(this.HitPlayerClip != null)
                    CameraSoundPlayer.Instance.PlayClipAtCam(this.HitPlayerClip);
            }
        }
    }
}
