using MusicApp.DTO;
namespace MusicApp.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string message, string type = null!);
        Task<List<NotificationDto>> GetNotificationsAsync(string userId);
        Task<string> MarkAsReadAsync(int notificationId, string userId);

    }
}
