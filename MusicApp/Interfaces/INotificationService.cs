namespace MusicApp.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string message, string type = null);
    }
}
