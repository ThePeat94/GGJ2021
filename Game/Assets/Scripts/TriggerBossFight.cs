using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossFight : MonoBehaviour
{
    [SerializeField] private MagmaBoss m_magmaBoss;
    [SerializeField] private PlayerHud m_playerHud;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            this.m_magmaBoss.StartFight();
            this.m_playerHud.ShowBossHud(this.m_magmaBoss);
        }
    }
}
