using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoarAttackCollider : MonoBehaviour
{

    [SerializeField] private float m_ticksPerSecond = 1f;

    private float m_currentTickCoolDown;

    private PlayerController m_hitPlayer;
    
    
    public int Damage { get; set; }

    private void Update()
    {
        if (this.m_currentTickCoolDown > 0f)
            this.m_currentTickCoolDown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        var hitPlayer = other.GetComponent<PlayerController>();
        if (hitPlayer != null)
        {
            this.m_hitPlayer = hitPlayer;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.m_currentTickCoolDown <= 0f && this.m_hitPlayer != null)
        {
            this.m_hitPlayer.HealthController.UseResource(Damage);
            this.m_currentTickCoolDown = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hitPlayer = other.GetComponent<PlayerController>();
        if (hitPlayer != null)
        {
            this.m_hitPlayer = null;
        }
    }
}
