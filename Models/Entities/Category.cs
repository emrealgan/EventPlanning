using EventPlanning.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Models.Entities
{
    public class Category
    {
        public int ID { get; set; }
        public string? Name { get; set; }

        // Navigation properties
        public ICollection<User>? Users { get; set; }
        public ICollection<Activity>? Activities { get; set; }
        public ICollection<ActivityCategory>? ActivityCategories { get; set; }

    }
}
