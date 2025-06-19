namespace MusicApp.DTO
{
    public class CreateArtistDto
    {
        public string Genre { get; set; } = string.Empty;
        public int ExperienceInYears { get; set; }
        public string? Bio { get; set; }
        public string? Availability { get; set; }
        public string? PortfolioUrl { get; set; }
    }
}
