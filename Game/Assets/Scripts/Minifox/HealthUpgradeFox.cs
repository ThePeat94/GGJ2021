using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgradeFox : Minifox
{
    [SerializeField] private int m_upgradeValue;

    private void Awake()
    {
        this.Upgrade = new HealthUpgrade(PlayerController.Instance, this.m_upgradeValue);
    }
}
