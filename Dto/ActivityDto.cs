namespace EventPlanning.Dto
{
    public class ActivityDto
    {
        public string? ActivityName { get; set; }
        public string? Description { get; set; }
        public int OwnerID { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Duration { get; set; }
        public int LocationID { get; set; }
    }
}
