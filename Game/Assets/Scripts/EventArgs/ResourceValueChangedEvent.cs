namespace Nidavellir.FoxIt.EventArgs
{
    public class ResourceValueChangedEvent : System.EventArgs
    {
        public ResourceValueChangedEvent(int newValue)
        {
            this.NewValue = newValue;
        }

        public int NewValue { get; }
    }
}