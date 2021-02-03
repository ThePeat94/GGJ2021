namespace Nidavellir.FoxIt.EventArgs
{
    public class ResourceValueChangedEventArgs : System.EventArgs
    {
        public ResourceValueChangedEventArgs(int newValue)
        {
            this.NewValue = newValue;
        }

        public int NewValue { get; }
    }
}