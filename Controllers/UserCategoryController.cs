using Microsoft.AspNetCore.Mvc;
using EventPlanning.Models.Entities;
using EventPlanning.Data;
using Microsoft.EntityFrameworkCore;
using EventPlanning.Dto;

[Route("api/[controller]")]
[ApiController]
public class UserCategoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserCategoryController> _logger;

    public UserCategoryController(AppDbContext context, ILogger<UserCategoryController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult> GetAllUserCategories()
    {
        var userCategories = await _context.UserCategories!
            .Include(uc => uc.UserID)
            .Include(uc => uc.CategoryID)
            .Select(uc => new UserCategory
            {
                UserID = uc.UserID,
                CategoryID = uc.CategoryID
            }).ToListAsync();

        return Ok(userCategories);
    }

    [HttpGet("{userID}")]
    public async Task<ActionResult> GetUserCategory(int userID)
    {
        var userCategory = await _context.UserCategories!
            .Where(uc => uc.UserID == userID)
            .GroupBy(uc => uc.UserID)
            .Select(group => new UserCategoryDto
            {
                UserID = group.Key,
                CategoryIDs = group.Select(uc => uc.CategoryID).ToList()
            })
            .FirstOrDefaultAsync();

        if (userCategory == null)
        {
            return NotFound();
        }

        return Ok(userCategory);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUserCategories([FromBody] UserCategoryDto userCategoriesDto)
    {
        if (userCategoriesDto?.CategoryIDs == null || !userCategoriesDto.CategoryIDs.Any())
        {
            return BadRequest("Invalid input.");
        }

        try
        {
            var existingUserCategories = await _context.UserCategories!
                .Where(uc => uc.UserID == userCategoriesDto.UserID)
                .ToListAsync();

            if (existingUserCategories.Any())
            {
                _context.UserCategories!.RemoveRange(existingUserCategories);
                await _context.SaveChangesAsync();
            }

            var newUserCategories = userCategoriesDto.CategoryIDs.Select(categoryID => new UserCategory
            {
                UserID = userCategoriesDto.UserID,
                CategoryID = categoryID
            }).ToList();

            _context.UserCategories!.AddRange(newUserCategories);
            await _context.SaveChangesAsync();

            var userEntity = await _context.Users!
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.ID == userCategoriesDto.UserID);

            if (userEntity == null)
            {
                return NotFound("User not found.");
            }

            userEntity.Categories = await _context.Categories!
                .Where(c => userCategoriesDto.CategoryIDs.Contains(c.ID))
                .ToListAsync();

            _context.Users!.Update(userEntity);
            await _context.SaveChangesAsync();

            var userDto = new GetUserDto
            {
                ID = userEntity.ID,
                Username = userEntity.Username,
                Email = userEntity.Email,
                Categories = userEntity.Categories?
                    .Select(c => new KeyValuePair<int, string>(c.ID, c.Name ?? ""))
                    .ToList() ?? new List<KeyValuePair<int, string>>()
            };

            return Ok(new { User = userDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }



    [HttpDelete("{userID}")]
    public async Task<IActionResult> DeleteUserCategory(int userID)
    {
        var userCategory = await _context.UserCategories!.FindAsync(userID);
        if (userCategory == null)
        {
            return NotFound();
        }

        _context.UserCategories.Remove(userCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserCategoryExists(int userID)
    {
        return _context.UserCategories!.Any(uc => uc.UserID == userID);
    }
}
