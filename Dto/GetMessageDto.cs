namespace EventPlanning.Dto
{
    public class GetMessageDto
    {
        public int ID {  get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentTime { get; set; }
    }
}
