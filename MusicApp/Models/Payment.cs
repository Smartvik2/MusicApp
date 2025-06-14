using System.ComponentModel.DataAnnotations;

namespace MusicApp.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int AppointmentId { get; set; }             // What the payment was for
        public decimal Amount { get; set; }                
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";    
        public string? TransactionId { get; set; }         
        public string? PaymentMethod { get; set; }         
        public ApplicationUser? User { get; set; }
        public Appointment? Appointment { get; set; }
        

    }
}
