using System.ComponentModel.DataAnnotations;

namespace MusicApp.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public string StripePaymentId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;  // Who made the payment
        public int AppointmentId { get; set; }             // What the payment was for
        public long Amount { get; set; }
        public string ArtistId { get; set; } = string.Empty; // Who the payment is for (artist)
        public string Currency { get; set; } = "USD";
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";    
        public string? TransactionId { get; set; }                
        public ApplicationUser? User { get; set; }
        public ApplicationUser? Artist { get; set; }
        public Appointment? Appointment { get; set; }
        

    }
}
