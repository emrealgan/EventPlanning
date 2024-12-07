namespace EventPlanning.Models.Entities
{
    public class UserCategory
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }

        public User? User { get; set; } // Reference to the related User
        public Category? Category { get; set; } // Reference to the related Category
    }

}
