using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;

namespace MusicApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public MessageService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Users can send messages to each other
        // This method creates a new message and notifies the receiver
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
            // Notify the receiver about the new message
            await _notificationService.SendNotificationAsync(
            dto.ReceiverId,
            $"New message from a user.",
            "Message");
            return true;
        }

        // Users can retrieve messages
        public async Task<List<MessageDto>> GetMessagesAsync(string userId1, string userId2)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

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
