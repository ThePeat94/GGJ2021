using UnityEngine;

namespace Nidavellir.FoxIt.Enemy
{
    public class BossCollider : MonoBehaviour
    {
        [SerializeField] private MagmaBoss m_magmaBoss;
        [SerializeField] private Collider m_collider;

        private void OnEnable()
        {
            this.m_collider.enabled = true;
        }

        private void OnDisable()
        {
            this.m_collider.enabled = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            var arrow = other.gameObject.GetComponent<Arrow>();
            if (arrow != null)
            {
                AudioSource.PlayClipAtPoint(this.m_magmaBoss.Data.HitByArrowSound, this.m_magmaBoss.transform.position, 0.33f);
                this.m_magmaBoss.HealthController.UseResource(arrow.Damage);
                Destroy(arrow.gameObject);
            }
        }
    }
}