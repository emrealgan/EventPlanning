using EventPlanning.Data;
using EventPlanning.Dto;
using EventPlanning.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllMessages()
        {
            var messages = await _context.Messages!.ToListAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetMessageById(int id)
        {
            var message = await _context.Messages!.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(message);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMessagesByUserId(int userId)
        {
            // Retrieve all messages where the sender or receiver is the given user ID
            var messages = await _context.Messages!
                                          .Where(m => m.SenderID == userId || m.ReceiverID == userId)
                                          .ToListAsync();

            // Map to DTOs if necessary
            var messageDtos = messages.Select(m => new GetMessageDto
            {
                ID = m.ID,
                SenderID = m.SenderID,
                ReceiverID = m.ReceiverID,
                MessageText = m.MessageText,
                SentTime = m.SentTime
            }).ToList();

            // Return the list (empty or with items)
            return Ok(messageDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(MessageDto addMessageDto)
        {
            var sender = await _context.Users!.FindAsync(addMessageDto.SenderID);
            if (sender == null)
            {
                return NotFound("Sender not found.");
            }

            var receiver = await _context.Activities!
                .Include(a => a.Messages)
                .FirstOrDefaultAsync(a => a.ID == addMessageDto.ReceiverID);

            if (receiver == null)
            {
                return NotFound("Receiver (Activity) not found.");
            }

            var messageEntity = new Message
            {
                SenderID = addMessageDto.SenderID,
                ReceiverID = addMessageDto.ReceiverID,
                MessageText = addMessageDto.MessageText,
                SentTime = DateTime.UtcNow.AddHours(3),
            };

            receiver.Messages!.Add(messageEntity);
            await _context.SaveChangesAsync();

            var response = new
            {
                messageEntity.ID,
                messageEntity.SenderID,
                messageEntity.ReceiverID,
                messageEntity.MessageText,
                messageEntity.SentTime
            };

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages!.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages?.Remove(message);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
