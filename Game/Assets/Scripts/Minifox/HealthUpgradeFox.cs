using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public class HealthUpgradeFox : Minifox
    {
        [SerializeField] private int m_upgradeValue;

        private void Awake()
        {
            this.Upgrade = new HealthUpgrade(PlayerController.Instance, this.m_upgradeValue);
        }
    }
}