namespace EventPlanning.Models.Entities
{
    public class Message
    {
        public int ID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentTime { get; set; }

        // Navigation properties
        public User? Sender { get; set; }
        public Activity? Receiver { get; set; }
    }
}
