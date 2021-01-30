using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadUpgradeFox : Minifox
{
    [SerializeField] private float m_upgradeValue;

    private void Awake()
    {
        this.Upgrade = new ReloadUpgrade(PlayerController.Instance, this.m_upgradeValue);
    }
}
