using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUpgradeFox : Minifox
{
    [SerializeField] private int m_upgradeValue;

    private void Awake()
    {
        this.Upgrade = new AttackUpgrade(PlayerController.Instance, this.m_upgradeValue);
    }
}
