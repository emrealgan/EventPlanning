using EventPlanning.Data;
using EventPlanning.Dto;
using EventPlanning.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllCategories()
        {
            List<Category> categories = await _context.Categories!.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories!.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult AddCategory(CategoryDto addCategoryDto)
        {
            var categoryEntity = new Category()
            {
                Name = addCategoryDto.Name
            };

            _context.Categories?.Add(categoryEntity);
            _context.SaveChanges();
            return Ok(categoryEntity);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories?.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories?.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
