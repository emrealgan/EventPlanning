using EventPlanning.Data;
using EventPlanning.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using MyLibrary = EventPlanning.Security.MyLibrary;
using Microsoft.EntityFrameworkCore;
using EventPlanning.Dto;
using EventPlanningWebApi.Dto;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users!.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users!
                .Include(u => u.Categories) // Ensure Categories are loaded
                .Include(u => u.Messages)
                .Include(u => u.Scores)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Map to DTO
            var userDto = new GetUserDto
            {   
                BirthDate = user.BirthDate,
                FirstName = user.FirstName,
                Gender = user.Gender,
                LastName = user.LastName,
                LocationID = user.LocationID,
                PhoneNumber = user.PhoneNumber,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                ID = user.ID,
                Username = user.Username,
                Email = user.Email,
                Categories = user.Categories?.Select(c => new KeyValuePair<int, string>(c.ID, c.Name ?? "")).ToList()
                              ?? new List<KeyValuePair<int, string>>(),
                Messages = user.Messages?.Select(m => new GetMessageDto
                {
                    ID = m.ID,
                    SenderID = m.SenderID,
                    ReceiverID = m.ReceiverID,
                    MessageText = m.MessageText,
                    SentTime = m.SentTime
                }).ToList() ?? new List<GetMessageDto>(),
                Scores = user.Scores?.Select(s => new GetScoreDto
                {
                    ID = s.ID,
                    EarnedDate = s.EarnedDate,
                    Points = s.Points
                }).ToList() ?? new List<GetScoreDto>()
            };

            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDto addUserDto)
        {
            try
            {
                var userEntity = new User()
                {
                    Username = addUserDto.Username,
                    Email = addUserDto.Email,
                    Password = MyLibrary.HashPassword(addUserDto.Password ?? string.Empty),
                    LocationID = addUserDto.LocationID,
                    FirstName = addUserDto.FirstName,
                    LastName = addUserDto.LastName,
                    BirthDate = addUserDto.BirthDate,
                    Gender = addUserDto.Gender,
                    PhoneNumber = addUserDto.PhoneNumber,
                    ProfilePhotoUrl = addUserDto.ProfilePhotoUrl
                };

                await _context.Users!.AddAsync(userEntity);

                await _context.SaveChangesAsync();

                var savedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == addUserDto.Username && u.Email == addUserDto.Email);

                if (savedUser == null)
                {
                    return BadRequest(new { Message = "User could not be found after saving." });
                }

                return Ok(new { Id = savedUser.ID });
            }
            catch (Exception ex)
            {
                var detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { Message = "An error occurred while saving the user.", Error = detailedError });
            }
        }

        [HttpPut("{id}")]
        public async Task <IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users!.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            user.LocationID = updateUserDto.LocationID;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.BirthDate = updateUserDto.BirthDate;
            user.Gender = updateUserDto.Gender;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.ProfilePhotoUrl = updateUserDto.ProfilePhotoUrl;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users!.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users?.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
