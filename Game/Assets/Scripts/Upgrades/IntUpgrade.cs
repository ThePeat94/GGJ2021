public abstract class IntUpgrade : Upgrade
{
    protected int m_upgradeValue;


    public IntUpgrade(PlayerController playerController, int upgradeValue) : base(playerController)
    {
        this.m_upgradeValue = upgradeValue;
    }
}
