using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public class AttackUpgradeFox : Minifox
    {
        [SerializeField] private int m_upgradeValue;

        private void Awake()
        {
            this.Upgrade = new AttackUpgrade(PlayerController.Instance, this.m_upgradeValue);
        }
    }
}