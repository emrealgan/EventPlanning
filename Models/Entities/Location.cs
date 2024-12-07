namespace EventPlanning.Models.Entities
{
    public class Location
    {
        public int ID { get; set; }
        public string? Name { get; set; }

        // Navigation properties
        public ICollection<User>? Users { get; set; }
        public ICollection<Activity>? Activities { get; set; }
    }

}
