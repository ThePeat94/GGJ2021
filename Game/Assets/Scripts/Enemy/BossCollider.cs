using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    [SerializeField] private MagmaBoss m_magmaboss;

    private void OnCollisionEnter(Collision other)
    {
        var arrow = other.gameObject.GetComponent<Arrow>();
        if (arrow != null)
        {
            this.m_magmaboss.HealthController.UseResource(arrow.Damage);
            Destroy(arrow.gameObject);
        }
    }
}
