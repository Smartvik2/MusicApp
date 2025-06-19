namespace MusicApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Recipient
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: type (e.g., Booking, Payment, etc.)
        public string Type { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}
