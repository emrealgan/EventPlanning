using EventPlanning.Data;
using EventPlanning.Dto;
using EventPlanning.Models.Entities;
using EventPlanningWebApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ParticipantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParticipantController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllParticipants()
        {
            var participants = await _context.Participants!.ToListAsync();
            return Ok(participants);
        }

        [HttpGet("{userId}")]
        public async Task <IActionResult> GetParticipant(int userId)
        {
            var participant = await _context.Participants!
                .Where(p => p.UserID == userId)
                .ToListAsync();

            if (participant == null)
            {
                return NotFound();
            }
            return Ok(participant);
        }


        [HttpPost]
        public async Task<IActionResult> AddParticipant(ParticipantDto addParticipantDto)
        {
            var participantEntity = new Participant()
            {
                UserID = addParticipantDto.UserID,
                ActivityID = addParticipantDto.ActivityID
            };

            // Kullanıcı zaten etkinliğe katılmış mı?
            var existingParticipant = await _context.Participants!
                .FirstOrDefaultAsync(p => p.UserID == addParticipantDto.UserID && p.ActivityID == addParticipantDto.ActivityID);

            if (existingParticipant != null)
            {
                return BadRequest("The user has already participated in this activity.");
            }

            // Kullanıcıyı etkinliğe ekle
            await _context.Participants!.AddAsync(participantEntity);

            // Etkinliği al
            var activityEntity = await _context.Activities!
                .Include(a => a.Categories)
                .Include(a => a.Participants)
                .FirstOrDefaultAsync(a => a.ID == addParticipantDto.ActivityID);

            if (activityEntity == null)
            {
                return NotFound("Aktivite bulunamadı.");
            }

            var userEntity = await _context.Users!
                .Include(u => u.Scores)
                .FirstOrDefaultAsync(u => u.ID == addParticipantDto.UserID);

            if (userEntity == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var participationScore = 10;
            var creationBonus = 15;  
            var firstParticipationBonus = 20;

            var hasAnyScore = userEntity.Scores!.Any();
            var totalPoints = participationScore;

            if (!hasAnyScore)
            {
                totalPoints += firstParticipationBonus;
            }

            if (activityEntity.OwnerID == userEntity.ID)
            {
                totalPoints += creationBonus;
            }

            var userScore = new Score
            {
                UserID = userEntity.ID,
                EarnedDate = DateTime.UtcNow.AddHours(3),
                Points = totalPoints
            };

            userEntity.Scores!.Add(userScore);

            await _context.SaveChangesAsync();

            var activityDto = new GetActivityDto
            {
                ID = activityEntity.ID,
                OwnerID = activityEntity.OwnerID,
                LocationID = activityEntity.LocationID,
                ActivityName = activityEntity.ActivityName,
                Description = activityEntity.Description,
                Date = activityEntity.Date,
                Time = activityEntity.Time,
                Duration = activityEntity.Duration,
                Categories = activityEntity.Categories?.Select(c => new KeyValuePair<int, string>(c.ID, c.Name ?? "")).ToList() ?? new List<KeyValuePair<int, string>>(),
                Participants = activityEntity.Participants?.Select(p => new GetParticipantDto
                {
                    UserID = p.UserID,
                    Username = p.User?.Username
                }).ToList() ?? new List<GetParticipantDto>()
            };

            return Ok(new
            {
                activity = activityDto,
                participant = new GetParticipantDto
                {
                    UserID = participantEntity.UserID,
                    Username = userEntity.Username
                },
                score = userScore.Points
            });
        }


        [HttpDelete("{userId}/{activityId}")]
        public async Task<IActionResult> DeleteParticipant(int userId, int activityId)
        {
            var participant = await _context.Participants!
                .FirstOrDefaultAsync(p => p.UserID == userId && p.ActivityID == activityId);
            if (participant == null)
            {
                return NotFound();
            }

            _context.Participants?.Remove(participant);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
