namespace Nidavellir.FoxIt.Upgrades
{
    public class ReloadUpgrade : FloatUpgrade
    {
        public ReloadUpgrade(PlayerController playerController, float upgradeValue) : base(playerController, upgradeValue)
        {
        }

        public override void ApplyUpgrade()
        {
            this.m_playerController.ReloadTime -= this.m_upgradeValue;
        }
    }
}