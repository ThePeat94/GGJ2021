using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    [SerializeField] private MagmaBoss m_magmaboss;
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
            AudioSource.PlayClipAtPoint(this.m_magmaboss.Data.HitByArrowSound, this.m_magmaboss.transform.position, 0.33f);
            this.m_magmaboss.HealthController.UseResource(arrow.Damage);
            Destroy(arrow.gameObject);
        }
    }
}
