namespace MusicApp.DTO
{
    public class PendingArtistDto
    {
        public int ArtistId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int ExperienceInYears { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string PortfolioUrl { get; set; } = string.Empty;
    }
}
