using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;

namespace MusicApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendNotificationAsync(string userId, string message, string type = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();
        }

        public async Task<string> MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return "Notification not found or access denied";

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return "Notification marked as read";
        }

    }
}
