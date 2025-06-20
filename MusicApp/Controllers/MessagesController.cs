using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using System.Security.Claims;

namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet("{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetMessages(string user1Id, string user2Id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != user1Id && currentUserId != user2Id)
                return Forbid("You are not authorized to access these messages");

            try
            {
                if (string.IsNullOrWhiteSpace(user1Id) || string.IsNullOrWhiteSpace(user2Id))
                    return BadRequest(new { message = "User IDs must be provided." });

                var messages = await _messageService.GetMessagesAsync(user1Id, user2Id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch messages");
                return StatusCode(500, new { message = "An error occurred while retrieving messages."});
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (senderId == null)
                    return Unauthorized(new { message = "Invalid sender identity." });

                if (string.IsNullOrWhiteSpace(dto.ReceiverId) || string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new { message = "ReceiverId and message content are required." });

                var success = await _messageService.SendMessageAsync(senderId, dto);
                return success
                    ? Ok(new { message = "Message sent successfully." })
                    : StatusCode(500, new { message = "Failed to send message." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the message.", error = ex.Message });
            }
        }
    }
}
