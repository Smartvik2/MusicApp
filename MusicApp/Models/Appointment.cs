using MusicApp.Enums;

namespace MusicApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }  
        public string UserId { get; set; } = string.Empty;
        public string ArtistId { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; } 
        public string? UserNote { get; set; }
        public string? ArtistNote { get; set; }
        public int DurationInMinutes { get; set; }
        public ApplicationUser? User { get; set; }
        public ApplicationUser? Artist { get; set; }
    }
}
