namespace EventArgs
{
    public class ResourceValueChangedEventArgs : System.EventArgs
    {
        public int NewValue { get; }
        
        public ResourceValueChangedEventArgs(int newValue)
        {
            this.NewValue = newValue;
        }
    }
}