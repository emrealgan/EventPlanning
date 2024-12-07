using EventPlanning.Data;
using EventPlanning.Dto;
using EventPlanning.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllLocations()
        {
            var locations = await _context.Locations!.ToListAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetLocationById(int id)
        {
            var location = await _context.Locations!.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }

        [HttpPost]
        public IActionResult AddLocation(LocationDto addLocationDto)
        {
            var locationEntity = new Location()
            {
                Name = addLocationDto.Name
            };

            _context.Locations?.Add(locationEntity);
            _context.SaveChanges();
            return Ok(locationEntity);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLocation(int id)
        {
            var location = _context.Locations?.Find(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations?.Remove(location);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
