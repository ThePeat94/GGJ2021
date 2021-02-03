using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public class ReloadUpgradeFox : Minifox
    {
        [SerializeField] private float m_upgradeValue;

        private void Awake()
        {
            this.Upgrade = new ReloadUpgrade(PlayerController.Instance, this.m_upgradeValue);
        }
    }
}