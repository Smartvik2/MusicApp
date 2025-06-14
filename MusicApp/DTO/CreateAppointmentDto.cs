namespace MusicApp.DTO
{
    public class CreateAppointmentDto
    {
        public string ArtistId { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
