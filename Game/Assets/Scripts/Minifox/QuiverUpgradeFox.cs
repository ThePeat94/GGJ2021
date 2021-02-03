using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public class QuiverUpgradeFox : Minifox
    {
        [SerializeField] private int m_upgradeValue;

        private void Awake()
        {
            this.Upgrade = new QuiverUpgrade(PlayerController.Instance, this.m_upgradeValue);
        }
    }
}