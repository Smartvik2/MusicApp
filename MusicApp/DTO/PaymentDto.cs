namespace MusicApp.DTO
{
    public class PaymentDto
    {
        public int AppointmentId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
    }
}
