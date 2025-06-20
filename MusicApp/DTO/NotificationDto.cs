namespace MusicApp.DTO
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; } 
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
