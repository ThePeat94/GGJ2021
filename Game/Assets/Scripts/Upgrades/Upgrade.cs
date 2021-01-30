using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade
{
    protected PlayerController m_playerController;

    public Upgrade(PlayerController playerController)
    {
        this.m_playerController = playerController;
    }
    
    public abstract void ApplyUpgrade();
}
