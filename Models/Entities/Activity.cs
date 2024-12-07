namespace EventPlanning.Models.Entities
{
    public class Activity
    {
        public int ID { get; set; }
        public int OwnerID { get; set; }
        public string? ActivityName { get; set; }
        public string? Description { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Duration { get; set; }
        public int LocationID { get; set; }

        // Navigation properties

        public ICollection<Participant>? Participants { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<ActivityCategory>? ActivityCategories { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
    