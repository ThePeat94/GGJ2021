public abstract class FloatUpgrade : Upgrade
{
    protected float m_upgradeValue;


    public FloatUpgrade(PlayerController playerController, float upgradeValue) : base(playerController)
    {
        this.m_upgradeValue = upgradeValue;
    }
}
