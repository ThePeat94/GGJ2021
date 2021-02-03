using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public class MovementSpeedUpgradeFox : Minifox
    {
        [SerializeField] private float m_upgradeValue;

        private void Awake()
        {
            this.Upgrade = new MovementSpeedUpgrade(PlayerController.Instance, this.m_upgradeValue);
        }
    }
}