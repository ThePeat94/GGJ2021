using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiverUpgradeFox : Minifox
{
    [SerializeField] private int m_upgradeValue;

    private void Awake()
    {
        this.Upgrade = new QuiverUpgrade(PlayerController.Instance, this.m_upgradeValue);
    }
}
