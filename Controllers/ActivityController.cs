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

    public class ActivityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _context.Activities!
                .Include(a => a.Categories)!
                .Include(a => a.Participants)!
                .ThenInclude(p => p.User)
                .ToListAsync();

            var activityDtos = activities.Select(a => new GetActivityDto
            {
                ID = a.ID,
                ActivityName = a.ActivityName,
                Description = a.Description,
                Date = a.Date,
                Time = a.Time,
                Duration = a.Duration,
                OwnerID = a.OwnerID,
                LocationID = a.LocationID,
                Categories = a.Categories!.Select(c => new KeyValuePair<int, string>(c.ID, c.Name ?? "")).ToList(),
                Participants = a.Participants!.Select(p => new GetParticipantDto
                {
                    UserID = p.UserID,
                    Username = p.User?.Username
                }).ToList()
            }).ToList();

            return Ok(activityDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _context.Activities!
                .Include(a => a.Categories)
                .Include(a => a.Messages)
                .Include(a => a.Participants ?? new List<Participant>())
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.ID == id);

            if (activity == null)
            {
                return NotFound($"Activity with ID {id} not found.");
            }

            var activityDto = new GetActivityDto
            {
                ID = activity.ID,
                ActivityName = activity.ActivityName,
                Description = activity.Description,
                Date = activity.Date,
                Time = activity.Time,
                Duration = activity.Duration,
                OwnerID = activity.OwnerID,
                LocationID = activity.LocationID,
                Categories = activity.Categories?.Select(c => new KeyValuePair<int, string>(c.ID, c.Name ?? "")).ToList() ?? new List<KeyValuePair<int, string>>(),
                Participants = activity.Participants?.Select(p => new GetParticipantDto
                {
                    UserID = p.UserID,
                    Username = p.User?.Username
                }).ToList() ?? new List<GetParticipantDto>(),
                Messages = activity.Messages?.Select(m => new GetMessageDto
                {
                    ID = m.ID,
                    SenderID = m.SenderID,
                    ReceiverID = m.ReceiverID,
                    MessageText = m.MessageText,
                    SentTime = m.SentTime
                }).ToList() ?? new List<GetMessageDto>()
            };

            return Ok(activityDto);
        }


        [HttpPost]
        public async Task<IActionResult> AddActivity(ActivityDto addActivityDto)
        {
            var activityEntity = new Activity()
            {
                ActivityName = addActivityDto.ActivityName,
                OwnerID = addActivityDto.OwnerID,
                Description = addActivityDto.Description,
                Date = addActivityDto.Date,
                Time = addActivityDto.Time,
                Duration = addActivityDto.Duration,
                LocationID = addActivityDto.LocationID,
            };

            await _context.Activities!.AddAsync(activityEntity);
            await _context.SaveChangesAsync();
            return Ok(activityEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, ActivityDto updateActivityDto)
        {
            var activity = await _context.Activities!.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            activity.ActivityName = updateActivityDto.ActivityName ?? activity.ActivityName;
            activity.Description = updateActivityDto.Description ?? activity.Description;
            activity.Date = updateActivityDto.Date ?? activity.Date;
            activity.Time = updateActivityDto.Time ?? activity.Time;
            activity.Duration = updateActivityDto.Duration ?? activity.Duration;
            activity.LocationID = updateActivityDto.LocationID;

            await _context.SaveChangesAsync();
            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities!.FindAsync(id);
            if (activity == null)
            {
                return NotFound($"Activity with ID {id} not found.");
            }

            // Remove related messages
            var messages = await _context.Messages!
                                          .Where(m => m.ReceiverID == id)
                                          .ToListAsync();
            _context.Messages!.RemoveRange(messages);

            var participants = await _context.Participants!
                                              .Where(p => p.ActivityID == id)
                                              .ToListAsync();
            _context.Participants!.RemoveRange(participants);

            var categories = await _context.ActivityCategories!
                                            .Where(c => c.ActivityID == id)
                                            .ToListAsync();
            _context.ActivityCategories!.RemoveRange(categories);

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
    }
