namespace Nidavellir.FoxIt.Upgrades
{
    public class AttackUpgrade : IntUpgrade
    {
        public AttackUpgrade(PlayerController playerController, int upgradeValue) : base(playerController, upgradeValue)
        {
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            this.m_playerController.AttackDamage += this.m_upgradeValue;
        }
    }
}