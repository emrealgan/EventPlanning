namespace EventPlanning.Models.Entities
{
    public class ActivityCategory
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }
        public int CategoryID { get; set; }

        public Activity? Activity { get; set; }
        public Category? Category { get; set; }
    }

}
