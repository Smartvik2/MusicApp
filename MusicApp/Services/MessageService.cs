using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;
using MusicApp.Interfaces;

namespace MusicApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendMessageAsync(string senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<MessageDto>> GetMessagesAsync(string userId1, string userId2)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync(); // Fetch entities first

            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                Timestamp = m.Timestamp
            }).ToList();

            return messageDtos;
        }

    }


}
