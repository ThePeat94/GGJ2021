namespace Nidavellir.FoxIt.EventArgs
{
    public class PlayerMadeChoiceEvent : System.EventArgs
    {
        public string ChoiceId { get; }

        public PlayerMadeChoiceEvent(string id)
        {
            this.ChoiceId = id;
        }
    }
}