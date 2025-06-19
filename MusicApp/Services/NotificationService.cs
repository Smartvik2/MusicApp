using MusicApp.Data;
using MusicApp.Models;
using MusicApp.Interfaces;

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
    }
}
