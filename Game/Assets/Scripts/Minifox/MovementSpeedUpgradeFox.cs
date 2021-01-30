using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedUpgradeFox : Minifox
{
    [SerializeField] private float m_upgradeValue;

    private void Awake()
    {
        this.Upgrade = new MovementSpeedUpgrade(PlayerController.Instance, this.m_upgradeValue);
    }
}
