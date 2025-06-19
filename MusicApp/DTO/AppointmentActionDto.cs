using MusicApp.Enums;

namespace MusicApp.DTO
{
    public class AppointmentActionDto
    {
        public int AppointmentId { get; set; }
        public AppointmentAction Action { get; set; }
        public string? ArtistNote { get; set; }
        public DateTime? NewTime { get; set; } //Incase it's rescheduled
    }
}
