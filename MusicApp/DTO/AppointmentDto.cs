namespace MusicApp.DTO
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public string UserId { get; set; } = null!;
        public string ArtistId { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Status { get; set; } = null!;
        public string? UserNote { get; set; }
        public string? ArtistNote { get; set; }
        public string? UserName { get; set; }        
        public string? ArtistName { get; set; }
    }
}
