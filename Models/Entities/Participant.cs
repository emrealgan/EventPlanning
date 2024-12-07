namespace EventPlanning.Models.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int ActivityID { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Activity? Activity { get; set; }


    }
}
