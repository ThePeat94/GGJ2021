namespace Nidavellir.FoxIt.Upgrades
{
    public abstract class Upgrade
    {
        protected PlayerController m_playerController;

        public Upgrade(PlayerController playerController)
        {
            this.m_playerController = playerController;
        }

        public virtual void ApplyUpgrade()
        {
            this.m_playerController.HealthController.ResetValue();
        }
    }
}