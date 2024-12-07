using Microsoft.AspNetCore.Mvc;
using EventPlanning.Models.Entities;
using EventPlanning.Data;
using Microsoft.EntityFrameworkCore;
using EventPlanning.Dto;

[Route("api/[controller]")]
[ApiController]
public class ActivityCategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActivityCategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActivityCategories()
    {
        var activityCategories = await _context.ActivityCategories!
            .Select(ac => new ActivityCategoryDto
            {
                ActivityID = ac.ActivityID,
                CategoryID = ac.CategoryID
            }).ToListAsync();

        return Ok(activityCategories);
    }

    [HttpGet("{activityID}")]
    public async Task<IActionResult> GetActivityCategory(int activityID)
    {
        var activityCategory = await _context.ActivityCategories!
            .Where(ac => ac.ActivityID == activityID)
            .Select(ac => new ActivityCategoryDto
            {
                ActivityID = ac.ActivityID,
                CategoryID = ac.CategoryID
            }).FirstOrDefaultAsync();

        if (activityCategory == null)
        {
            return NotFound();
        }

        return Ok(activityCategory);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivityCategory(ActivityCategoryDto activityCategoryDto)
    {
        var activity = await _context.Activities!
            .Include(a => a.ActivityCategories)!
            .ThenInclude(ac => ac.Category)
            .FirstOrDefaultAsync(a => a.ID == activityCategoryDto.ActivityID);

        if (activity == null)
        {
            return NotFound($"Activity with ID {activityCategoryDto.ActivityID} not found.");
        }

        var category = await _context.Categories!
            .FirstOrDefaultAsync(c => c.ID == activityCategoryDto.CategoryID);

        if (category == null)
        {
            return NotFound($"Category with ID {activityCategoryDto.CategoryID} not found.");
        }

        if (activity.ActivityCategories!.Any(ac => ac.CategoryID == activityCategoryDto.CategoryID))
        {
            return BadRequest($"Category with ID {activityCategoryDto.CategoryID} is already associated with Activity ID {activityCategoryDto.ActivityID}.");
        }

        var activityCategory = new ActivityCategory
        {
            ActivityID = activityCategoryDto.ActivityID,
            CategoryID = activityCategoryDto.CategoryID
        };

        await _context.ActivityCategories!.AddAsync(activityCategory);

        await _context.SaveChangesAsync();

        return Ok($"Category with ID {activityCategoryDto.CategoryID} successfully associated with Activity ID {activityCategoryDto.ActivityID}.");
    }



    [HttpPut("{activityID}")]
    public async Task<IActionResult> UpdateActivityCategory(int activityID, ActivityCategoryDto activityCategoryDto)
    {
        if (activityID != activityCategoryDto.ActivityID)
        {
            return BadRequest();
        }

        var activityCategory = await _context.ActivityCategories!.FindAsync(activityID);
        if (activityCategory == null)
        {
            return NotFound();
        }

        activityCategory.CategoryID = activityCategoryDto.CategoryID;

        _context.Entry(activityCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ActivityCategoryExists(activityID))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{activityID}")]
    public async Task<IActionResult> DeleteActivityCategory(int activityID)
    {
        var activityCategory = await _context.ActivityCategories!.FindAsync(activityID);
        if (activityCategory == null)
        {
            return NotFound();
        }

        _context.ActivityCategories.Remove(activityCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ActivityCategoryExists(int activityID)
    {
        return _context.ActivityCategories!.Any(ac => ac.ActivityID == activityID);
    }
}
