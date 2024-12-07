using EventPlanning.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenHandler = EventPlanning.Security.TokenHandler;
using MyLibrary = EventPlanning.Security.MyLibrary;
using EventPlanning.Dto;
using EventPlanningWebApi.Dto;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AuthController(AppDbContext dbContext, IConfiguration configuration)
        {
            this._context = dbContext;
            _configuration = configuration;
        }

        [HttpPost("send-reset-code")]
        public async Task<IActionResult> SendResetCode([FromBody] SendCodeDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var resetCode = MyLibrary.GenerateResetCode();
            await _context.SaveChangesAsync();

            await MyLibrary.SendMail(request.Email, resetCode);
            return Ok(new { code = resetCode });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
            {
                return BadRequest("New password is required.");
            }

            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found or invalid reset code." });
            }

            var hashedPassword = MyLibrary.HashPassword(resetPasswordDto.NewPassword);
            Console.WriteLine($"Hashed Password: {hashedPassword}");
            user.Password = hashedPassword;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Password reset successfully." });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
            {
                return BadRequest("New password is required.");
            }
            if (changePasswordDto.CurrentPassword.IsNullOrEmpty())
            {
                return Unauthorized(new { Message = "Password cannot be empty" });
            }
            var hashedPassword = MyLibrary.HashPassword(changePasswordDto.CurrentPassword ?? "");

            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Password == hashedPassword);
            if (user == null)
            {
                return NotFound(new { Message = "User not found or invalid reset code." });
            }

            var newPassword = MyLibrary.HashPassword(changePasswordDto.NewPassword);
            Console.WriteLine($"Hashed Password: {hashedPassword}");
            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Password reset successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Password is required.");
            }

            if (_context.Users == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database connection error.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || string.IsNullOrWhiteSpace(user.Password) ||
                !MyLibrary.VerifyPassword(loginDto.Password, user.Password))
            {
                return Unauthorized(new { Message = "Valid username or password" });
            }

            string role = user.Username == "admin" ? "admin" : "user";
            var token = TokenHandler.CreateToken(_configuration, role, user.ID.ToString());
            return Ok(new { Token = token, Id = user.ID });
        }


        [HttpPost("existuser")]
        public IActionResult ExistUser([FromBody] ExistUserDto existUserDto)
        {
            if (existUserDto == null ||
                (string.IsNullOrWhiteSpace(existUserDto.Email) && string.IsNullOrWhiteSpace(existUserDto.Username)))
            {
                return BadRequest("Email or Username must be provided.");
            }

            var userExists = _context.Users!.Any(u =>
                u.Email == existUserDto.Email || u.Username == existUserDto.Username);

            if (userExists)
            {
                return Ok(new { exists = true, message = "User already exists." });
            }
            else
            {
                return Ok(new { exists = false, message = "User does not exist." });
            }
        }
    }
}
