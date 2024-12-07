namespace EventPlanning.Models.Entities
{
    public class Score
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int Points { get; set; }
        public DateTime? EarnedDate { get; set; }

    }
}
