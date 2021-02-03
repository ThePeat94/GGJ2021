using UnityEngine;

namespace Nidavellir.FoxIt
{
    public class Arrow : MonoBehaviour
    {
        private Collider m_collider;

        private bool m_hasHit;
        private Rigidbody m_rigidbody;

        public int Damage { get; set; }

        private void Awake()
        {
            this.m_collider = this.GetComponent<Collider>();
            this.m_rigidbody = this.GetComponent<Rigidbody>();
            Destroy(this.gameObject, 5f);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!this.m_hasHit)
                this.transform.rotation = Quaternion.LookRotation(this.m_rigidbody.velocity);
        }

        private void OnCollisionEnter(Collision other)
        {
            this.m_rigidbody.velocity = Vector3.zero;
            this.m_rigidbody.isKinematic = true;
            this.transform.SetParent(other.transform);
            this.m_hasHit = true;
            this.m_collider.enabled = false;
        }
    }
}