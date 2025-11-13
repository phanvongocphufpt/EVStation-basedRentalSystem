using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;

namespace EVStation_basedRentalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly EVSDbContext _dbContext;
        private readonly ILogger<ChatController> _logger;

        public ChatController(EVSDbContext dbContext, ILogger<ChatController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // 📩 Gửi tin nhắn
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Message))
                return BadRequest("Tin nhắn không được để trống.");

            message.SentAt = DateTime.Now;

            _dbContext.ChatMessages.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(message);
        }

        // 💬 Lấy lịch sử chat giữa 2 người
        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(int user1Id, int user2Id)
        {
            var messages = await _dbContext.ChatMessages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
