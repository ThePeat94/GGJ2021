using UnityEngine;

namespace Nidavellir.FoxIt.Enemy
{
    public class BossAttackCollider : MonoBehaviour
    {
        public int Damage { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            var hitPlayer = other.GetComponent<PlayerController>();
            if (hitPlayer != null)
            {
                Debug.Log("BOSS HIT PLAYER");
                hitPlayer.HealthController.UseResource(this.Damage);
            }
        }
    }
}