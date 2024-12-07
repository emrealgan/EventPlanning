using EventPlanning.Models.Entities;
using EventPlanning.Dto;

namespace EventPlanningWebApi.Dto
{
    public class GetActivityDto
    {
        public int ID { get; set; }
        public string? ActivityName { get; set; }
        public string? Description { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Duration { get; set; }
        public int OwnerID { get; set; }
        public int LocationID { get; set; }

        // Sadece kategori isimleri
        public List<KeyValuePair<int, string>>? Categories { get; set; }
        public List<GetParticipantDto>? Participants { get; set; }
        public List<GetMessageDto>? Messages { get; set; }


    }
}
