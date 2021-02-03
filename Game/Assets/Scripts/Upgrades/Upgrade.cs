namespace Nidavellir.FoxIt.Upgrades
{
    public abstract class Upgrade
    {
        protected PlayerController m_playerController;

        public Upgrade(PlayerController playerController)
        {
            this.m_playerController = playerController;
        }

        public abstract void ApplyUpgrade();
    }
}