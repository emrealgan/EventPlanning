using EventPlanning.Data;
using EventPlanning.Dto;
using EventPlanning.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ScoreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScoreController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllScores()
        {
            var scores = await _context.Scores!.ToListAsync();
            return Ok(scores);
        }

        [HttpGet("{userId}")]
        public async Task <IActionResult> GetScoresByUserId(int userId)
        {
            var score = await _context.Scores!.Where(s => s.UserID == userId).ToListAsync();
            if (score == null || !score.Any())
            {
                return NotFound();
            }
            return Ok(score);
        }

        [HttpPost]
        public async Task <IActionResult> AddScore(ScoreDto addScoreDto)
        {
            var scoreEntity = new Score()
            {
                UserID = addScoreDto.UserID,
                Points = addScoreDto.Points,
                EarnedDate = DateTime.UtcNow
            };

            await _context.Scores!.AddAsync(scoreEntity);
            await _context.SaveChangesAsync();
            return Ok(scoreEntity);
        }

        [HttpDelete("{userId}")]
        public async Task <IActionResult> DeleteScore(int userId)
        {
            var score = await _context.Scores!.FirstOrDefaultAsync(s => s.UserID == userId);
            if (score == null)
            {
                return NotFound();
            }

            _context.Scores?.Remove(score);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
