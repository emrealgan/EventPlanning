namespace EventPlanning.Dto
{
    public class MessageDto
    {
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentTime { get; set; }

    }
}
