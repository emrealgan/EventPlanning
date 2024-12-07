namespace EventPlanning.Dto
{
    public class UserCategoryDto
    {
        public int UserID { get; set; }
        public List<int> CategoryIDs { get; set; } = new();
    }
}
