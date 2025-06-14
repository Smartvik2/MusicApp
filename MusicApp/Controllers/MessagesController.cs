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

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetMessages(string user1Id, string user2Id)
        {
            var messages = await _messageService.GetMessagesAsync(user1Id, user2Id);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return Unauthorized();

            var result = await _messageService.SendMessageAsync(senderId, dto);
            return result ? Ok("Message sent") : BadRequest("Failed to send");
        }
    }
}
