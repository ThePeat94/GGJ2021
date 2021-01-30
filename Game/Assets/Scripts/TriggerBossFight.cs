using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossFight : MonoBehaviour
{
    [SerializeField] private MagmaBoss m_magmaBoss;
    [SerializeField] private PlayerHud m_playerHud;

    private void Start()
    {
        this.m_magmaBoss.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            this.m_magmaBoss.gameObject.SetActive(true);
            this.m_magmaBoss.StartFight();
            this.m_playerHud.ShowBossHud(this.m_magmaBoss);
            Destroy(this.gameObject);
        }
    }
}
