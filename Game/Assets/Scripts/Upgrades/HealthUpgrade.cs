public class HealthUpgrade : IntUpgrade
{
    public HealthUpgrade(PlayerController playerController, int upgradeValue) : base(playerController, upgradeValue)
    {
    }

    public override void ApplyUpgrade()
    {
        this.m_playerController.HealthController.IncreaseMaximum(this.m_upgradeValue);
    }
}
