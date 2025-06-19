namespace MusicApp.DTO
{
    public class PaymentHistoryDto
    {
        public long Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
