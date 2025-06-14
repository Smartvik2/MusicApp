namespace MusicApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }  
        public string UserId { get; set; } = string.Empty;
        public string ArtistId { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string? Notes { get; set; }
        public int DurationInMinutes { get; set; }
        public string Status { get; set; } = "Pending";
        public ApplicationUser? User { get; set; }
        public ApplicationUser? Artist { get; set; }
    }
}
