namespace EventPlanning.Models.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int LocationID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePhotoUrl { get; set; }

        // Navigation properties

        public ICollection<Activity>? Activities { get; set; }
        public ICollection<Participant>? Participants { get; set; }
        public ICollection<Score>? Scores { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<UserCategory>? UserCategories { get; set; }

    }
}
